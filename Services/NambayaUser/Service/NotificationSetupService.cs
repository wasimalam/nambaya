using Common.BusinessObjects;
using Common.Infrastructure;
using Common.Infrastructure.Exceptions;
using Common.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NambayaUser.Contracts.Interfaces;
using System;
using System.Collections.Generic;
using System.Text.Json;
using UserManagement.Contracts.Models;

namespace NambayaUser.Service
{
    public class NotificationSetupService : BaseService, INotificationSetupWrapperService
    {
        private readonly WebServiceConfiguration _webApiConf;

        public NotificationSetupService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _webApiConf = serviceProvider.GetRequiredService<WebServiceConfiguration>();
        }
        public NotificationTemplateBO AddTemplate(NotificationTemplateBO notificationTemplateBO)
        {
            try
            {
                var sessionContext = GetSessionContext();
                if(sessionContext.RoleCode != RoleCodes.NambayaUser)
                    throw new ServiceException(ClientSideErrors.INVALID_USER_ACTION);
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.UserManagementServiceBaseUrl))
                {
                    notificationTemplateBO.CreatedBy = sessionContext.LoginName;
                    var res = apiClient.InternalServicePostAsync("api/v1/notification/notificationtemplates", JsonSerializer.Serialize(notificationTemplateBO)).Result;
                    return JsonSerializer.Deserialize<NotificationTemplateBO>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }
        }

        public IEnumerable<LookupsBO> GetNotificationEventTypes()
        {
            using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.UserManagementServiceBaseUrl))
            {
                var res = apiClient.InternalServiceGetAsync($"api/v1/notification/notificationeventtypes").Result;
                return JsonSerializer.Deserialize<IEnumerable<LookupsBO>>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            }
        }

        public IEnumerable<NotificationEventParamBO> GetNotificationParamsByEvent(long eventid)
        {
            using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.UserManagementServiceBaseUrl))
            {
                var res = apiClient.InternalServiceGetAsync($"api/v1/notification/eventparams/{eventid}").Result;
                return JsonSerializer.Deserialize<IEnumerable<NotificationEventParamBO>>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            }
        }

        public NotificationTemplateBO GetNotificationTemplate(long id)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.UserManagementServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/notification/notificationtemplates/{id}").Result;
                    return JsonSerializer.Deserialize<NotificationTemplateBO>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message == ClientSideErrors.RESOURCE_NOT_FOUND)
                    return null;
                throw ex.InnerException;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        public NotificationTemplateBO GetNotificationTemplate(long eventtypeid, long templatetypeid)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.UserManagementServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/notification/notificationtemplates/eventtype?eventtypeid={eventtypeid}&templatetypeid={templatetypeid}").Result;
                    return JsonSerializer.Deserialize<NotificationTemplateBO>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message == ClientSideErrors.RESOURCE_NOT_FOUND)
                    return null;
                throw ex.InnerException;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        public PagedResults<NotificationTemplateBO> GetNotificationTemplates(int limit, int offset, string orderby, string param)
        {
            using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.UserManagementServiceBaseUrl))
            {
                if (string.IsNullOrWhiteSpace(orderby))
                    orderby = "CreatedOn desc";
                var res = apiClient.InternalServiceGetAsync($"api/v1/notification/notificationtemplates?offset={offset}&limit={limit}&orderby={orderby}&filter={param}").Result;
                return JsonSerializer.Deserialize<PagedResults<NotificationTemplateBO>>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            }
        }

        public IEnumerable<LookupsBO> GetNotificationTemplateTypes()
        {
            using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.UserManagementServiceBaseUrl))
            {
                var res = apiClient.InternalServiceGetAsync($"api/v1/notification/notificationtemplatetypes").Result;
                return JsonSerializer.Deserialize<IEnumerable<LookupsBO>>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            }
        }

        public IEnumerable<NotificationEventTypeBO> GetNotificationTypesByEvent(long eventid)
        {
            using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.UserManagementServiceBaseUrl))
            {
                var res = apiClient.InternalServiceGetAsync($"api/v1/notification/notificationtypes/{eventid}").Result;
                return JsonSerializer.Deserialize<IEnumerable<NotificationEventTypeBO>>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            }
        }

        public NotificationTemplateBO UpdateTemplate(NotificationTemplateBO notificationTemplateBO)
        {
            try
            {
                var sessionContext = GetSessionContext();
                notificationTemplateBO.UpdatedBy = sessionContext.LoginName;
                if (sessionContext.RoleCode != RoleCodes.NambayaUser)
                    throw new ServiceException(ClientSideErrors.INVALID_USER_ACTION);
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.UserManagementServiceBaseUrl))
                {
                    var res = apiClient.InternalServicePutAsync("api/v1/notification/notificationtemplates", notificationTemplateBO).Result;
                    return JsonSerializer.Deserialize<NotificationTemplateBO>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }
        }
    }
}
