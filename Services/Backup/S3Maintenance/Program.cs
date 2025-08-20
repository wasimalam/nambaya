using Common.Infrastructure;
using Common.Infrastructure.Extensions;
using GreenPipes;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using S3Maintenance.Consumers;
using S3Maintenance.Services;
using Serilog;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
namespace S3Maintenance
{
    internal class Program
    {
        public static RabbitMQSettings rabbitMQSettings { get; set; }
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        private static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .Enrich.FromLogContext()
                .CreateLogger();

            try
            {
                Log.Information("Worker host launching...");
                var isService = !(Debugger.IsAttached || args.Contains("--console"));
                var s3Configuration = new S3Configuration();
                var ftpConfiguration = new FTPConfiguration();

                var builder = new HostBuilder()
                    .ConfigureAppConfiguration((hostingContext, config) =>
                    {
                        config.AddJsonFile("appsettings.json", optional: true);
                        config.AddEnvironmentVariables();

                        if (args != null)
                            config.AddCommandLine(args);
                        config.Build().GetSection("S3Configuration").Bind(s3Configuration);
                        config.Build().GetSection("FTPConfiguration").Bind(ftpConfiguration);
                    })
                     .ConfigureServices((hostContext, services) =>
                     {
                         services.AddHostedService<S3MaintenanceService>();
                         services.Configure<RabbitMQSettings>(hostContext.Configuration.GetSection("RabbitMQSettings"));
                         //services.AddMassTransitSupport(Configuration);
                         services.AddSingleton(_ => s3Configuration);
                         services.AddSingleton(_ => ftpConfiguration);
                         services.AddSingleton(typeof(S3MaintenanceService), typeof(S3MaintenanceService));
                         services.AddTransient(typeof(BackupService), typeof(BackupService));
                         services.AddTransient(typeof(IFTPService), typeof(FTPService));
                         services.AddTransient(typeof(IFileSyncService), typeof(SyncService));
                         services.AddTransient(typeof(RabbitMQClient), typeof(RabbitMQClient));
                         services.AddMassTransit(cfg =>
                         {
                             cfg.AddConsumer<FileEventConsumer>();
                             cfg.AddBus(ConfigureBus);
                         });
                     })
                    .ConfigureLogging((hostingContext, logging) =>
                    {
                        logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                        logging.AddConsole();
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
                cfg.ReceiveEndpoint(KnownChannels.FILE_SYNC_EVENT_CHANNEL, ep =>
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
                    ep.ConfigureConsumer<FileEventConsumer>(provider);
                });

            });
        }
    }
}
