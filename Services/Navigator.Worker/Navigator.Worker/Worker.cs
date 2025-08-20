using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Navigator.Worker
{
    public class Worker : IHostedService
    {
        private readonly IBusControl _bus;
        private readonly ILogger _logger;
        private Timer _timer;
        private readonly ScheduleConfiguration _scheduleConfiguration;
        private readonly RestartNavigatorConfiguration _restartNavigatorConfiguration;
        private readonly RestartTestLeftConfiguration _restartTestLeftConfiguration;
        public static bool ProcessingPatient { get; set; }
        public static bool ProcessingRestart { get; set; }
        public Worker(ScheduleConfiguration scheduleConfiguration, RestartNavigatorConfiguration restartNavigatorConfiguration, RestartTestLeftConfiguration restartTestLeftConfiguration,
            IBusControl bus, ILoggerFactory loggerFactory)
        {
            _bus = bus;
            _logger = loggerFactory.CreateLogger<Worker>();
            _scheduleConfiguration = scheduleConfiguration;
            _restartNavigatorConfiguration = restartNavigatorConfiguration;
            _restartTestLeftConfiguration = restartTestLeftConfiguration;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting bus");
            _bus.StartAsync(cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult();

            TimeSpan interval = TimeSpan.FromHours(24);
            var firstInterval = CalculateFirstInterval(_scheduleConfiguration.ScheduleSameDay ? 0 : 1);
            Action action = () =>
            {
                if (firstInterval.TotalSeconds < 0)
                {
                    RestartNavigator(null);
                    firstInterval = CalculateFirstInterval(1);
                }
                //now schedule it to be called every 24 hours for future
                // timer repeates call to RemoveScheduledAccounts every 24 hours.
                _timer = new Timer(
                    RestartNavigator,
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
            _logger.LogInformation("Stopping bus");
            _bus.StopAsync(cancellationToken);
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
        private TimeSpan CalculateFirstInterval(int dayOffset = 0)
        {
            var nextRunTime = DateTime.Today.AddDays(dayOffset).AddHours(_scheduleConfiguration.ScheduleAtHour)
              .AddMinutes(_scheduleConfiguration.ScheduleAtMinute);
            var curTime = DateTime.Now;
            return nextRunTime.Subtract(curTime);
        }
        private void RestartNavigator(object state)
        {
            while (ProcessingPatient == true)
            {
                Console.WriteLine ("Waiting for ProcessingPatient");
                Thread.Sleep(TimeSpan.FromSeconds(10));
            }
            ProcessingRestart = true;
            using Process process = new Process
            {
                EnableRaisingEvents = false,
                StartInfo = new ProcessStartInfo
                {
                    //WorkingDirectory = _navigatorConfiguration.JrePath,
                    UseShellExecute = false,
                    FileName = "java",
                    Arguments = "-jar " + '"' + _restartNavigatorConfiguration.ExecutableJarPath + "\""
                                                + " \"" + _restartNavigatorConfiguration.UserName + "\""
                                                + " \"" + _restartNavigatorConfiguration.Password + "\"",
                    RedirectStandardOutput = true, //Set output of program to be written to process output stream
                    RedirectStandardError = true
                }
            };
            if (process.Start())
            {
                _logger.LogInformation($"Process Launched to restart navigator");
                process.WaitForExit();
            }
            ProcessingRestart = false;
            _logger.LogInformation("Leaving RestartNavigator");
        }

        private void RestartTestLeft(object state)
        {
            while (ProcessingPatient == true)
            {
                Console.WriteLine("Waiting for ProcessingPatient");
                Thread.Sleep(TimeSpan.FromSeconds(10));
            }
            ProcessingRestart = true;
            Process.GetProcesses().FirstOrDefault(pr => pr.ProcessName == _restartTestLeftConfiguration.ExecutableName).CloseMainWindow();

            using Process process = new Process
            {
                EnableRaisingEvents = false,
                StartInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    FileName = _restartTestLeftConfiguration.ExecutablePath + "\\" + _restartTestLeftConfiguration.ExecutableName,
                    RedirectStandardOutput = true, //Set output of program to be written to process output stream
                    RedirectStandardError = true
                }
            };
            if (process.Start())
            {
                _logger.LogInformation($"Process Launched to restart testleft");
                process.WaitForExit();
            }
            ProcessingRestart = false;
            _logger.LogInformation("Leaving Restart TestLeft");
        }
    }
}