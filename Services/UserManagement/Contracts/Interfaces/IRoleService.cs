using System.Collections.Generic;
using UserManagement.Contracts.Models;

namespace UserManagement.Contracts.Interfaces
{
    public interface IRoleService
    {
        RoleBO GetRoleByLoginName(string loginname);
        List<RoleBO> GetRoles(string applicationCode);
    }
}
