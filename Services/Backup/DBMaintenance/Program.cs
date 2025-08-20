using Common.Infrastructure;
using Common.Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
namespace DBMaintenance
{
    internal class Program
    {

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
                var dbConfiguration = new DBConfiguration();
                var ftpConfiguration = new FTPConfiguration();

                var builder = new HostBuilder()
                    .ConfigureAppConfiguration((hostingContext, config) =>
                    {
                        config.AddJsonFile("appsettings.json", optional: true);
                        config.AddEnvironmentVariables();

                        if (args != null)
                            config.AddCommandLine(args);
                        config.Build().GetSection("DBConfiguration").Bind(dbConfiguration);
                        config.Build().GetSection("FTPConfiguration").Bind(ftpConfiguration);
                    })
                     .ConfigureServices((hostContext, services) =>
                     {
                         services.AddHostedService<DBMaintenanceService>();
                         services.AddMassTransitSupport(Configuration);
                         services.AddSingleton(_ => dbConfiguration);
                         services.AddSingleton(_ => ftpConfiguration);
                         services.AddSingleton(typeof(DBMaintenanceService), typeof(DBMaintenanceService));
                         services.AddTransient(typeof(BackupService), typeof(BackupService));
                         services.AddTransient(typeof(RetentionService), typeof(RetentionService));
                         services.AddTransient(typeof(RabbitMQClient), typeof(RabbitMQClient));
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
    }
}
