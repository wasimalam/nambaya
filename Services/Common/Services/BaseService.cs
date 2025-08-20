using Common.BusinessObjects;
using Common.Infrastructure;
using Common.Services.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Common.Services
{
    public class BaseService
    {
        protected readonly IServiceProvider _serviceProvider;
        public BaseService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public SessionContext GetSessionContext()
        {
            try
            {
                IHttpContextAccessor httpContextAccessor = _serviceProvider.GetRequiredService<IHttpContextAccessor>();
                    return httpContextAccessor.HttpContext.GetSessionContext();
            }
            catch
            {
            }
            return null;
        }
        public string GetCorrelationId()
        {
            try
            {
                IHttpContextAccessor httpContextAccessor = _serviceProvider.GetRequiredService<IHttpContextAccessor>();
                return httpContextAccessor.HttpContext?.Items[LoggingConstants.CorrelationId]?.ToString();
            }
            catch
            {
            }
            return null;
        }
    }
}
