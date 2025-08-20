using Common.DataAccess.Interfaces;
using System.Collections.Generic;
using UserManagement.Repository.Models;

namespace UserManagement.Repository.Interfaces
{
    public interface IUserRepository : IDapperRepositoryBase<User>
    {
        User GetByLoginName(string loginId);
        IEnumerable<User> GetByRoleCode(string rolecode, bool? isActive, bool? isLocked);
        IEnumerable<User> GetByLoginName(List<string> loginId);
        IEnumerable<User> GetUserForWelcomeMessage();
    }
}