using Common.DataAccess;
using Common.DataAccess.Interfaces;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UserManagement.Repository.Interfaces;
using UserManagement.Repository.Models;

namespace UserManagement.Repository.Repositories
{
    public class UserRepository : DapperRepositoryBase<User>, IUserRepository
    {
        public UserRepository(IDatabaseSession connectionFactory) : base(connectionFactory)
        {
        }
        public User GetByLoginName(string loginId)
        {
            var user = GetItems(System.Data.CommandType.Text, $"select u.*, ur.RoleId as RoleID, r.Code as Role  FROM [user] u " +
                $"inner join UserRole ur on u.ID = ur.UserID inner join [Role] r  on ur.RoleID = r.ID where loginName = @loginId", new { loginId = loginId }).FirstOrDefault();
            return user;
        }
        public IEnumerable<User> GetByRoleCode(string rolecode, bool? isActive, bool? isLocked)
        {
            return GetItems(System.Data.CommandType.Text, $"SELECT u.*, ur.RoleId as RoleID, '{rolecode}' as Role  FROM [user] u inner join UserRole ur on u.ID = ur.UserID" +
                $" inner join [Role] r on ur.RoleID = r.ID where r.Code=@rolecode {(isActive != null ? " and u.IsActive=@isActive" : "")}{(isLocked != null ? " and u.IsLocked = @isLocked" : "")}", new { rolecode, isActive, isLocked });
        }
        public IEnumerable<User> GetUserForWelcomeMessage()
        {
            return GetItems(System.Data.CommandType.Text, $"SELECT * FROM [user] where isWelcomeMessageRequired = 1");
        }
        public IEnumerable<User> GetByLoginName(List<string> loginIds)
        {
            StringBuilder sb = new StringBuilder();
            int i = 1;
            DynamicParameters dynamicParameters = new DynamicParameters();
            foreach (var loginid in loginIds)
            {
                // IN clause
                sb.Append("@loginId" + i.ToString() + ",");
                // parameter
                dynamicParameters.Add("@loginId" + i.ToString(), loginid);
                i++;
            }
            sb = sb.Remove(sb.Length - 1, 1);
            return GetItems(System.Data.CommandType.Text, $"select u.*, ur.RoleId as RoleID, r.Code as Role from [user] u" +
                $" inner join UserRole ur on u.ID = ur.UserID inner join [Role] r  on ur.RoleID = r.ID where loginName in ({sb})", dynamicParameters);
        }
    }
}
