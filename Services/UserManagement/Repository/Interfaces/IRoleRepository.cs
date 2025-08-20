using System;
using System.Collections.Generic;
using System.Text;
using Common.DataAccess.Interfaces;
using UserManagement.Repository.Models;

namespace UserManagement.Repository.Interfaces
{
    public interface IRoleRepository : IDapperRepositoryBase<Role>
    {
        IEnumerable<Role> GetByApplicationCode(string applicationCode);
        Role GetRole(string applicationCode, string roleCode);
        Role GetByLoginName(string loginName);
    }
}
