using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DBMaintenance
{
    public class DBMaintenanceService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly DBConfiguration _dBConfiguration;
        private Timer _timer;
        private readonly ILogger<DBMaintenanceService> _logger;

        public DBMaintenanceService(IServiceProvider serviceProvider, ILogger<DBMaintenanceService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _dBConfiguration = _serviceProvider.GetRequiredService<DBConfiguration>();
            _logger.LogInformation("DBMaintenanceService constructor");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("DBMaintenanceService StartAsync");
            //TimeSpan interval = TimeSpan.FromSeconds(20); //For testing
            TimeSpan interval = TimeSpan.FromHours(24);
            var firstInterval = CalculateFirstInterval(_dBConfiguration.ScheduleSameDay ? 0 : 1);
            Action action = () =>
            {
                if (firstInterval.TotalSeconds < 0)
                {
                    RunBackup(null);
                    firstInterval = CalculateFirstInterval(1);
                }
                //now schedule it to be called every 24 hours for future
                // timer repeates call to RemoveScheduledAccounts every 24 hours.
                _timer = new Timer(
                    RunBackup,
                    null,
                    firstInterval,
                    interval
                );
            };

            // no need to await this call here because this task is scheduled to run much much later.
            Task.Run(action);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private void RunBackup(object state)
        {
            _logger.LogInformation("DBMaintenanceService RunBackup");
            //var t1 = Task.Delay(new TimeSpan(0,0,10));//For testing
            //t1.Wait();//For testing           
            var backupService = _serviceProvider.GetRequiredService<BackupService>();
            backupService.DoWork();
            var retentionService = _serviceProvider.GetRequiredService<RetentionService>();
            retentionService.DoWork();
        }
        //calculate time to run the first time & delay to set the timer
        //DateTime.Today gives time of midnight 00.00
        private TimeSpan CalculateFirstInterval(int dayOffset = 0)
        {
            var nextRunTime = DateTime.Today.AddDays(dayOffset).AddHours(_dBConfiguration.ScheduleAtHour)
              .AddMinutes(_dBConfiguration.ScheduleAtMinute);
            var curTime = DateTime.Now;
            return nextRunTime.Subtract(curTime);
        }
    }
}
