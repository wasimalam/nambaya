using Common.Infrastructure;
using Common.Services;
using Logging.Contracts.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;

namespace Notification.API
{
    public class LoggingService : ILoggingService
    {
        protected readonly IServiceProvider _serviceProvider;
        protected readonly WebServiceConfiguration _webApiConf;
        protected readonly ILogger<ILoggingService> _logger;
        public LoggingService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _webApiConf = serviceProvider.GetRequiredService<WebServiceConfiguration>();
            _logger = serviceProvider.GetRequiredService<ILogger<ILoggingService>>();
        }
        public void LogNotification(Common.BusinessObjects.NotificationBO notificationBO, long templateTypeId)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.LoggingServiceBaseUrl))
                {
                    var res = apiClient.InternalServicePostAsync("api/v1/logging/lognotifications",
                        JsonSerializer.Serialize(
                            new NotificationsBO()
                            {
                                EventTypeId = notificationBO.EventTypeId,
                                TemplateTypeId = templateTypeId,
                                Message = notificationBO.Body,
                                Subject = notificationBO.Subject,
                                Recipient = notificationBO.Address,
                                TimeStamp = DateTime.UtcNow
                            })).Result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"LogNotification error: {ex.Message}", ex);
            }
        }
    }
}
