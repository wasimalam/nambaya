using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace S3Maintenance.Services
{
    public class S3MaintenanceService : IHostedService
    {
        private readonly IBusControl _bus;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<S3MaintenanceService> _logger;

        public S3MaintenanceService(IServiceProvider serviceProvider, ILogger<S3MaintenanceService> logger, IBusControl bus)
        {
            _bus = bus;
            _serviceProvider = serviceProvider;
            _logger = logger;
            _logger.LogInformation("S3MaintenanceService constructor");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var conf = _serviceProvider.GetService<IConfiguration>();
            _logger.LogInformation("S3MaintenanceService StartAsync");
            _logger.LogInformation("Starting bus");
            _bus.StartAsync(cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult();
            if (conf.GetValue<bool>("DoBackup") == true)

            {
                Action action = () =>
            {
                RunBackup(null);
            };

                // no need to await this call here because this task is scheduled to run much much later.
                Task.Run(action);
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void RunBackup(object state)
        {
            _logger.LogInformation("S3MaintenanceService RunBackup");
            var backupService = _serviceProvider.GetRequiredService<BackupService>();
            backupService.DoWork();
        }
    }
}
