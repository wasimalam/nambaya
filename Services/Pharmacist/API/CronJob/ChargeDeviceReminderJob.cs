using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Interfaces.Interfaces;
using Common.Services.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pharmacist.Contracts.Interfaces;

namespace Pharmacist.API.CronJob
{
    public class ChargeDeviceReminderJob : CronJobService
    {
        private readonly ILogger<ChargeDeviceReminderJob> _logger;
        private readonly IServiceProvider _serviceProvider;

        public ChargeDeviceReminderJob(ICronConfig<ChargeDeviceReminderJob> config, ILogger<ChargeDeviceReminderJob> logger, IServiceProvider serviceProvider)
            : base(config.CronExpression, config.TimeZoneInfo)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(StartAsync)}: About to schedule device reminder job.");
            return base.StartAsync(cancellationToken);
        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now:hh:mm:ss} About to send reminder for charging device to pharmacist.");
            try
            {
                var scope = _serviceProvider.CreateScope();
                var notificationService = scope.ServiceProvider.GetRequiredService<IPharmacistNotificationService>();
                notificationService.RemindChargingDevice();
            }
            catch (Exception e)
            {
                _logger.LogError($"{nameof(DoWork)}: Error occurred while sending device reminder. Error: {e.Message}");
            }
            
            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(StopAsync)}: About to stop device reminder job.");
            return base.StopAsync(cancellationToken);
        }
    }
}
