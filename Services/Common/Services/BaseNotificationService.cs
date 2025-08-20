using Common.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.Json;

namespace Common.Services
{
    public class BaseNotificationService
    {
        protected readonly IServiceProvider _serviceProvider;
        protected readonly WebServiceConfiguration _webApiConf;
        protected Func<string, string, string, string, string> FillUserTemplate = (message, username, userphone, useremail) =>
        {
            return message.Replace(NotificationConstants.USERNAME, username ?? "")
               .Replace(NotificationConstants.USERPHONE, userphone ?? "")
               .Replace(NotificationConstants.USEREMAIL, useremail);
        };
        protected Func<string, string, string, string, string, string> FillUserOTPTemplate = (message, username, userphone, useremail, otp) =>
        {
            return message.Replace(NotificationConstants.USERNAME, username ?? "")
               .Replace(NotificationConstants.USERPHONE, userphone ?? "")
               .Replace(NotificationConstants.USEREMAIL, useremail)
               .Replace(NotificationConstants.OTPCODE, otp);
        };
        protected Func<string, string, string, string, string, string> FillUserPasswordTemplate = (message, username, userphone, useremail, password) =>
        {
            return message.Replace(NotificationConstants.USERNAME, username ?? "")
               .Replace(NotificationConstants.USERPHONE, userphone ?? "")
               .Replace(NotificationConstants.USEREMAIL, useremail)
               .Replace(NotificationConstants.PASSWORD, password);
        };
        public BaseNotificationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _webApiConf = serviceProvider.GetRequiredService<WebServiceConfiguration>();
        }
        protected T GetNotificationTemplate<T>(long eventtypeid, long templatetypeid)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.UserManagementServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/notification/notificationtemplates/eventtype?eventtypeid={eventtypeid}&templatetypeid={templatetypeid}").Result;
                    return JsonSerializer.Deserialize<T>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch
            {
                return default(T);
            }
        }
        protected T GetNotificationEvent<T>(long eventtypeid)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.UserManagementServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/notification/notificationtypes/{eventtypeid}").Result;
                    return JsonSerializer.Deserialize<T>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch
            {
                return default(T);
            }
        }
        protected string GetUserApplicationUIUrl(string applicationCode)
        {
            if (applicationCode == ApplicationNames.PharmacistApp)
                return _webApiConf.PharmacistUIUrl;
            else if (applicationCode == ApplicationNames.CardiologistApp)
                return _webApiConf.CardiologistUIUrl;
            else if (applicationCode == ApplicationNames.CentralGroupApp)
                return _webApiConf.CentralGroupUIUrl;
            else if (applicationCode == ApplicationNames.NamabayaUserApp)
                return _webApiConf.NambayaUserUIUrl;
            else
                throw new NotImplementedException();
        }
    }
}
