using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Common.Infrastructure.LogContext
{
    public class LogHeaderMiddleware
    {
        private readonly RequestDelegate _next;

        public LogHeaderMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var header = context.Request.Headers[LoggingConstants.CorrelationId];
            string sessionId;

            if (header.Count > 0)
            {
                sessionId = header[0];
            }
            else
            {
                sessionId = Guid.NewGuid().ToString();
            }
            context.Items[LoggingConstants.CorrelationId] = sessionId;
            var logger = context.RequestServices.GetRequiredService<ILogger<LogHeaderMiddleware>>();
            using (logger.BeginScope($"@{{{LoggingConstants.CorrelationId}}}", sessionId))
            {
                await this._next(context);
            }
        }
    }
}
