using System;
using Common.Interfaces.Interfaces;
using Common.Services.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Services.Extensions
{
    public static class CronJobExtension
    {
        public static IServiceCollection AddCronJob<T>(this IServiceCollection services, Action<ICronConfig<T>> options) where T : CronJobService
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options), @"Please provide Schedule Configurations.");
            }
            var config = new CronConfig<T>();;
            options.Invoke(config);
            if (string.IsNullOrWhiteSpace(config.CronExpression))
            {
                throw new ArgumentNullException(nameof(CronConfig<T>.CronExpression), @"Empty Cron Expression is not allowed.");
            }

            services.AddSingleton<ICronConfig<T>>(config);
            services.AddHostedService<T>();
            return services;
        }
    }
}
