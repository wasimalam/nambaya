using Common.BusinessObjects;
using Common.Infrastructure;
using Common.Services;
using Identity.Contracts.Interfaces;
using Identity.Repository.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using UserManagement.Contracts.Models;

namespace Identity.Service
{
    public class IdentityService : IIdentityService
    {
        private readonly ApiRequestConfiguration _apiSecretConf;
        private readonly WebServiceConfiguration _webApiConf;
        private readonly IHostRegistryRepository _hostsRepository;
        private readonly ILogger<IdentityService> _logger;
        private readonly IServiceProvider _serviceProvider;
        public IdentityService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _hostsRepository = serviceProvider.GetRequiredService<IHostRegistryRepository>();
            _webApiConf = serviceProvider.GetRequiredService<WebServiceConfiguration>();
            _apiSecretConf = serviceProvider.GetRequiredService<ApiRequestConfiguration>();
            _logger = serviceProvider.GetRequiredService<ILogger<IdentityService>>();
        }
        string IIdentityService.GetErrorUrl()
        {
            return _hostsRepository.GetErrorUrl();
        }
        string IIdentityService.GetLogInUrl()
        {
            return _hostsRepository.GetLogInUrl();
        }
        string IIdentityService.GetLogOutUrl()
        {
            return _hostsRepository.GetLogOutUrl();
        }
        string IIdentityService.GetIdentityServerUrl()
        {
            return _hostsRepository.GetIdentityServerUrl();
        }
        string[] IIdentityService.GetCorsUrls()
        {
            return _hostsRepository.GetCorsUrls();
        }
        SessionContext IIdentityService.IsValidUser(string username, string password, List<string> roles)
        {
            try
            {
                _logger.LogTrace($"Entered IIdentityService.IsValidUser username {username} password {password} roles {roles}");
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.UserManagementServiceBaseUrl))
                {
                    var res = apiClient.InternalServicePostAsync("api/v1/user/IsValidUser", JsonSerializer.Serialize(new
                    {
                        LoginId = username,
                        Password = password
                    })).Result;
                    return JsonSerializer.Deserialize<SessionContext>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                var e = ex.InnerException ?? ex;
                if (e.Message.Contains(ClientSideErrors.USER_ID_INACTIVE))
                    throw new Common.Infrastructure.Exceptions.ServiceException(ClientSideErrors.USER_ID_INACTIVE);
                else if (e.Message.Contains(ClientSideErrors.USER_ID_LOCKED))
                    throw new Common.Infrastructure.Exceptions.ServiceException(ClientSideErrors.USER_ID_LOCKED);
                else if (e.Message.Contains(ClientSideErrors.USER_ID_DELETED))
                    throw new Common.Infrastructure.Exceptions.ServiceException(ClientSideErrors.USER_ID_DELETED);
                else if (e.Message.Contains(ClientSideErrors.INVALID_USER_ID_PASSWORD))
                    throw new Common.Infrastructure.Exceptions.ServiceException(ClientSideErrors.INVALID_USER_ID_PASSWORD);
                else if (e.Message.Contains(ClientSideErrors.PASSWORD_RESET_REQUIRED))
                {
                    var role = GetRole(username);

                    if (roles.Any(p => p.Trim().ToLower() == role.Code.ToLower()) == false)
                    {
                        throw new Common.Infrastructure.Exceptions.ServiceException(ClientSideErrors.INVALID_USER_ID_PASSWORD);
                    }

                    if (role.Code == RoleCodes.Pharmacist)
                    {
                        var pharmacy = GetParentPharmacy(username);
                        if (pharmacy == null || !pharmacy.IsActive || pharmacy.IsLocked)
                            throw new Common.Infrastructure.Exceptions.ServiceException(ClientSideErrors.PHARMACY_IS_NOT_ACCESSIBLE);
                    }
                    throw new Common.Infrastructure.Exceptions.ServiceException(ClientSideErrors.PASSWORD_RESET_REQUIRED);
                }
                _logger.LogTrace($"IIdentityService.IsValidUser exception {e.Message} {e.StackTrace}");
                throw e;
            }
        }

        SessionContext IIdentityService.GetUMUser(long id)
        {
            try
            {
                _logger.LogTrace($"Entered IIdentityService.GetUmUser {id}");
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.UserManagementServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/user/context/{id}").Result;
                    return JsonSerializer.Deserialize<SessionContext>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                var e = ex.InnerException ?? ex;
                _logger.LogTrace($"IIdentityService.GetUMUser exception {e.Message} {e.StackTrace}");
                throw e;
            }
        }
        UserSettingBO IIdentityService.GetUserSetting(string loginname, string code)
        {
            try
            {
                _logger.LogTrace($"Entered IIdentityService.GetUserSetting");
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.UserManagementServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/user/settings/{loginname}").Result;
                    var resultobj = JsonSerializer.Deserialize<List<UserSettingBO>>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    return resultobj.FirstOrDefault(p => p.Code == code);
                }
            }
            catch (Exception ex)
            {
                var e = ex.InnerException ?? ex;
                _logger.LogTrace($"IIdentityService.GetUserSetting exception {e.Message} {e.StackTrace}");
                throw e;
            }
        }

        public NotificationTemplateBO GetNotificationTemplate(long eventtypeid, long templatetypeid)
        {
            try
            {
                _logger.LogTrace($"Entered IIdentityService.GetNotificationTemplate");
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.UserManagementServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/notification/notificationtemplates/eventtype?eventtypeid={eventtypeid}&templatetypeid={templatetypeid}").Result;
                    return JsonSerializer.Deserialize<NotificationTemplateBO>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                var e = ex.InnerException ?? ex;
                _logger.LogTrace($"IIdentityService.GetNotificationTemplate exception {e.Message} {e.StackTrace}");
                throw e;
            }
        }
        private BaseUserBO GetParentPharmacy(string pharmacistemail)
        {
            string webApiUrl = _webApiConf.PharmacistServiceBaseUrl;
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, webApiUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/pharmacy/pharmacist/{pharmacistemail}").Result;
                    return JsonSerializer.Deserialize<BaseUserBO>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw ex.InnerException ?? ex;
            }
        }
        void IIdentityService.GetUser(SessionContext umUser)
        {
            GetAppUser(umUser);
        }
        private void GetAppUser(SessionContext umUser)
        {
            string webApiUrl;
            string apiController = umUser.RoleCode;
            if (umUser.ApplicationCode == ApplicationNames.PharmacistApp)
                webApiUrl = _webApiConf.PharmacistServiceBaseUrl;
            else if (umUser.ApplicationCode == ApplicationNames.CardiologistApp)
                webApiUrl = _webApiConf.CardiologistServiceBaseUrl;
            else if (umUser.ApplicationCode == ApplicationNames.CentralGroupApp)
                webApiUrl = _webApiConf.CentralGroupServiceBaseUrl;
            else if (umUser.ApplicationCode == ApplicationNames.NamabayaUserApp)
            {
                webApiUrl = _webApiConf.NambayaUserServiceBaseUrl;
                apiController = RoleCodes.NambayaUser;
            }
            else
                throw new NotImplementedException();
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, webApiUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/{apiController}/email/" + umUser.LoginName).Result;
                    var appUser = JsonSerializer.Deserialize<BasicUser>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    if (appUser != null)
                    {
                        umUser.AppUserID = appUser.ID;
                        umUser.FirstName = appUser.FirstName ?? appUser.Name;
                        umUser.LastName = appUser.LastName;
                        umUser.Phone = appUser.Phone;
                        umUser.PharmacyID = appUser.PharmacyID;
                        umUser.CardiologistID = appUser.CardiologistID;
                    }
                    if (umUser.RoleCode == RoleCodes.Pharmacist)
                    {
                        var parentSession = GetUMUser(appUser.CreatedBy);
                        if (parentSession == null || !parentSession.IsActive || parentSession.IsLocked || parentSession.IsDeleted)
                            throw new Common.Infrastructure.Exceptions.ServiceException(ClientSideErrors.PHARMACY_IS_NOT_ACCESSIBLE);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw ex.InnerException ?? ex;
            }
        }
        private RoleBO GetRole(string loginname)
        {
            try
            {
                _logger.LogTrace($"Entered GetRole by loginname {loginname}");
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.UserManagementServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/role/loginname/{loginname}").Result;
                    return JsonSerializer.Deserialize<RoleBO>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                var e = ex.InnerException ?? ex;
                _logger.LogTrace($" Exception {e.Message} {e.StackTrace}");
                return null;
            }
        }
        public SessionContext GetUMUser(string loginname)
        {
            try
            {
                _logger.LogTrace($"Entered IIdentityService.GetUmUser by loginname {loginname}");
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.UserManagementServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/user/context/email/{loginname}").Result;
                    return JsonSerializer.Deserialize<SessionContext>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                var e = ex.InnerException ?? ex;
                _logger.LogTrace($" Exception {e.Message} {e.StackTrace}");
                return null;
            }
        }
        public bool ChangePassword(string loginname, string newpassword, string applicationcode)
        {
            try
            {
                _logger.LogTrace($"Entered IIdentityService.GetUmUser by loginname");
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.UserManagementServiceBaseUrl))
                {
                    var res = apiClient.InternalServicePostAsync($"api/v1/user/UpdateCredentials", JsonSerializer.Serialize(new
                    {
                        LoginId = loginname,
                        Password = newpassword,
                        ApplicationId = applicationcode
                    })).Result;
                    _logger.LogInformation($"Password changed sucessfully");
                    return true;
                }
            }
            catch (Exception ex)
            {
                var e = ex.InnerException ?? ex;
                _logger.LogTrace($" Exception {e.Message} {e.StackTrace}");
                throw e;
            }
        }
    }
    internal class BasicUser
    {
        public long ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public long PharmacyID { get; set; }
        public long CardiologistID { get; set; }
        public string CreatedBy { get; set; }

    }
}
