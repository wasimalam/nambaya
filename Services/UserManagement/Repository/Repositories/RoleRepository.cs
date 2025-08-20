using Common.DataAccess;
using Common.DataAccess.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UserManagement.Repository.Interfaces;
using UserManagement.Repository.Models;

namespace UserManagement.Repository.Repositories
{
    public class RoleRepository : DapperRepositoryBase<Role>, IRoleRepository
    {
        public RoleRepository(IDatabaseSession databaseSession) : base(databaseSession)
        {
        }

        public IEnumerable<Role> GetByApplicationCode(string applicationCode)
        {
            return GetItems(commandType: CommandType.Text,
                $"select r.* from {TableName} r inner join [Application] a on r.ApplicationID=a.ID where a.Code=@applicationCode",
                new { applicationCode = applicationCode });
        }

        public Role GetByLoginName(string loginName)
        {
            return GetItems(commandType: CommandType.Text,
                 $"select r.* from {TableName} r inner join UserRole ur on r.ID=ur.RoleID inner join [User] u on u.ID=ur.UserID where u.LoginName=@loginName",
                 new { loginname = loginName }).FirstOrDefault();
        }

        public Role GetRole(string applicationCode, string roleCode)
        {
            return GetItems(CommandType.Text,
                 "select r.* from [Application] a inner join [Role] r on a.Id = R.ApplicationID where a.Code=@applicationCode and r.Code=@role",
                 new { applicationCode = applicationCode, role = roleCode }).FirstOrDefault();
        }
    }
}
