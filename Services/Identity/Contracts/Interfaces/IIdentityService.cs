using System.Collections.Generic;
using Common.BusinessObjects;
using Common.Infrastructure;
using UserManagement.Contracts.Models;

namespace Identity.Contracts.Interfaces
{
    public interface IIdentityService
    {
        string GetLogInUrl();
        string GetLogOutUrl();
        string GetErrorUrl();
        string GetIdentityServerUrl();
        string[] GetCorsUrls();
        SessionContext IsValidUser(string username, string password, List<string> roles);
        SessionContext GetUMUser(long id);
        SessionContext GetUMUser(string username);
        UserSettingBO GetUserSetting(string loginname, string code = UserSettingCodes.FACTOR_NOTIFICATION_TYPE);
        void GetUser(SessionContext umUser);
        bool ChangePassword(string loginname, string newpassword, string applicationcode);
        NotificationTemplateBO GetNotificationTemplate(long eventtypeid, long templatetypeid);
    }
}
