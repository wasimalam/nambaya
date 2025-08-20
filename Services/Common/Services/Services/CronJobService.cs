using System;
using System.Threading;
using System.Threading.Tasks;
using Cronos;
using Microsoft.Extensions.Hosting;

namespace Common.Services.Services
{
    public abstract class CronJobService : IHostedService, IDisposable
    {
        private System.Threading.Timer _timer;
        private readonly CronExpression _expression;
        private readonly TimeZoneInfo _timeZoneInfo;

        protected CronJobService(string cronExpression, TimeZoneInfo timeZoneInfo)
        {
            _expression = CronExpression.Parse(cronExpression);
            _timeZoneInfo = timeZoneInfo;
        }

        public virtual async Task StartAsync(CancellationToken cancellationToken)
        {
            await ScheduleJob(cancellationToken);
        }

        protected virtual async Task ScheduleJob(CancellationToken cancellationToken)
        {
            var next = _expression.GetNextOccurrence(DateTimeOffset.Now, _timeZoneInfo);
            if (next.HasValue)
            {
                var delay = next.Value - DateTimeOffset.Now;
                if (delay.TotalMilliseconds <= 0)  
                {
                    await ScheduleJob(cancellationToken);
                }
                _timer = new System.Threading.Timer(
                    async (cancellationToken) =>
                    {
                        _timer?.Dispose();  
                        _timer = null;

                        if (!((CancellationToken)cancellationToken).IsCancellationRequested)
                        {
                            await DoWork(((CancellationToken)cancellationToken));
                        }

                        if (!((CancellationToken)cancellationToken).IsCancellationRequested)
                        {
                            await ScheduleJob(((CancellationToken)cancellationToken));  
                        }
                    }, cancellationToken, (long)delay.TotalMilliseconds, (long)delay.TotalMilliseconds);
            }
            await Task.CompletedTask;
        }

        public virtual async Task DoWork(CancellationToken cancellationToken)
        {
            await Task.Delay(5000, cancellationToken);  
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Dispose();
            await Task.CompletedTask;
        }

        public virtual void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
