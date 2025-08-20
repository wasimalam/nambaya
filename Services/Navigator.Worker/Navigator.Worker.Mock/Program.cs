using Common.Infrastructure;
using Common.Services.Handlers;
using GreenPipes;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Navigator.Worker.Mock.Consumer;
using Serilog;
using Serilog.Sinks.RabbitMQ;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;


namespace Navigator.Worker.Mock
{
    internal class Program
    {
        public static RabbitMQSettings rabbitMQSettings { get; set; }

        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            //.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        private static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .Enrich.FromLogContext()
                .WriteTo.RabbitMQ((clientConfiguration, sinkConfiguration) =>
                {
                    clientConfiguration.Username = Configuration["RabbitMQSettings:UserName"];
                    clientConfiguration.Password = Configuration["RabbitMQSettings:Password"];
                    clientConfiguration.Exchange = SerilogRabbitMQ.Exchange;
                    clientConfiguration.ExchangeType = "fanout";
                    clientConfiguration.DeliveryMode = RabbitMQDeliveryMode.Durable;
                    clientConfiguration.RouteKey = SerilogRabbitMQ.Routing;
                    clientConfiguration.Port = ushort.Parse(Configuration["RabbitMQSettings:Port"]);
                    clientConfiguration.VHost = Configuration["RabbitMQSettings:VirtualHost"];
                    clientConfiguration.Hostnames.Add(Configuration["RabbitMQSettings:Host"]);
                    if (bool.Parse(Configuration["RabbitMQSettings:SSLActive"]))
                    {
                        clientConfiguration.SslOption = new RabbitMQ.Client.SslOption()
                        {
                            Enabled = bool.Parse(Configuration["RabbitMQSettings:SSLActive"]),
                            CertPath = Configuration["RabbitMQSettings:SSLCertificatePath"],
                            Version = SslProtocols.Tls12,
                            AcceptablePolicyErrors = SslPolicyErrors.RemoteCertificateNameMismatch |
                                                    SslPolicyErrors.RemoteCertificateChainErrors
                        };
                    }
                    sinkConfiguration.TextFormatter = new Serilog.Formatting.Compact.CompactJsonFormatter();
                })
                .CreateLogger();

            try
            {
                Log.Information("Worker host launching...");
                var isService = !(Debugger.IsAttached || args.Contains("--console"));

                var builder = new HostBuilder()
                    .ConfigureAppConfiguration((hostingContext, config) =>
                    {
                        config.AddJsonFile("appsettings.json", optional: true);
                        config.AddEnvironmentVariables();

                        if (args != null)
                            config.AddCommandLine(args);
                    })
                    .ConfigureServices((hostContext, services) =>
                    {
                        services.Configure<RabbitMQSettings>(hostContext.Configuration.GetSection("RabbitMQSettings"));
                        services.AddTransient<RequestHandler>();
                        services.AddTransient<SecureServiceHandler>();

                        services.AddMassTransit(cfg =>
                        {
                            cfg.AddConsumer<PatientConsumer>();
                            cfg.AddBus(ConfigureBus);
                        });
                        services.AddHostedService<Worker>();

                    })
                    .ConfigureLogging((hostingContext, logging) =>
                    {
                        logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                        logging.AddConsole();
                    })
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseStartup<Startup>();
                    });

                if (isService)
                {
                    await builder
                        .UseWindowsService()
                        .UseSerilog()
                        .Build()
                        .RunAsync();
                    //await builder.UseSystemd().Build().RunAsync(); // For Linux, replace the nuget package: "Microsoft.Extensions.Hosting.WindowsServices" with "Microsoft.Extensions.Hosting.Systemd", and then use this line instead
                }
                else
                {
                    await builder
                        .UseSerilog()
                        .RunConsoleAsync();
                }
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly", ex);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IBusControl ConfigureBus(IServiceProvider provider)
        {
            rabbitMQSettings = provider.GetRequiredService<IOptions<RabbitMQSettings>>().Value;
            return Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host =
                cfg.Host(rabbitMQSettings.Host, rabbitMQSettings.Port, rabbitMQSettings.VirtualHost, h =>
                {
                    h.Username(rabbitMQSettings.Username);
                    h.Password(rabbitMQSettings.Password);
                    if (rabbitMQSettings.SSLActive)
                    {
                        h.UseSsl(ssl =>
                        {
                            ssl.Protocol = SslProtocols.Tls12;
                            ssl.Certificate = new X509Certificate2(rabbitMQSettings.SSLCertificatePath);
                            ssl.AllowPolicyErrors(SslPolicyErrors.RemoteCertificateNameMismatch | SslPolicyErrors.RemoteCertificateChainErrors);
                            ssl.UseCertificateAsAuthenticationIdentity = false;
                        });
                    }
                });
                cfg.UseMessageScheduler(new Uri($"rabbitmq://{rabbitMQSettings.Host}:{rabbitMQSettings.Port}{rabbitMQSettings.VirtualHost}{rabbitMQSettings.Quartz}"));
                cfg.ReceiveEndpoint(KnownChannels.NAVIGATOR_EDF_FILE_UPLOADED_EVENT_CHANNEL, ep =>
                {
                    ep.PrefetchCount = 1;
                    ep.UseScheduledRedelivery(r => r.Intervals(TimeSpan.FromMinutes(15),
                                                                TimeSpan.FromMinutes(30),
                                                                TimeSpan.FromHours(1),
                                                                TimeSpan.FromHours(2),
                                                                TimeSpan.FromHours(4),
                                                                TimeSpan.FromHours(8),
                                                                TimeSpan.FromHours(12),
                                                                TimeSpan.FromHours(16),
                                                                TimeSpan.FromDays(1),
                                                                TimeSpan.FromDays(7)));
                    ep.UseMessageRetry(r =>
                    {
                        r.Interval(5, TimeSpan.FromSeconds(30));
                    });
                    //ep.UseMessageRetry(r =>
                    //{
                    //    r.Interval(5, TimeSpan.FromSeconds(30));
                    //    //r.Handle<ServiceException>(x => x.Message.Contains("Licenses"));
                    //});
                    ep.ConfigureConsumer<PatientConsumer>(provider);
                });
                //cfg.UseDelayedExchangeMessageScheduler();
            });
        }
    }

}