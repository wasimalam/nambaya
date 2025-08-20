using Common.BusinessObjects;
using Common.Infrastructure;
using System.Collections.Generic;
using UserManagement.Contracts.Models;

namespace UserManagement.Contracts.Interfaces
{
    public interface IUserService
    {
        PagedResults<UserBO> GetUsers(int limit, int offset, string orderby, string param);
        UserBO GetUserById(long id);
        SessionContext GetUserSessionById(long id);
        SessionContext GetUserSessionByLoginId(string loginname);
        UserBO GetUserByLoginName(string loginname);
        List<UserBO> GetUsersByRole(string role, bool? isActive, bool? isLocked);
        List<UserBO> GetUsersByLoginName(string[] loginnames);
        SessionContext IsValidUser(string loginId, string password, bool bLoginAttemp);
        UserBO CreateUser(BaseUserBO baseUser);
        void UpdateUserCredentials(BaseUserBO baseUser);
        void UpdateUser(BaseUserBO baseUser);
        List<UserSettingBO> GetUserSettings(string loginid);
        void UpateUserSettings(string loginid, IEnumerable<UserSettingBO> userSettings);
        void ChangeCredentials(string loginId, ChangeCredentialBO req);
    }
}
