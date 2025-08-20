using Common.DataAccess;
using Common.DataAccess.Interfaces;
using Common.Infrastructure;
using Common.Infrastructure.Exceptions;
using Dapper;
using System.Data;
using System.Linq;
using UserManagement.Repository.Interfaces;
using UserManagement.Repository.Models;

namespace UserManagement.Repository.Repositories
{
    public class UserRoleRepository : DapperRepositoryBase<UserRole>, IUserRoleRepository
    {
        public UserRoleRepository(IDatabaseSession databaseSession) : base(databaseSession)
        {
        }
        public UserRole UpdateUserRole(UserRole userRole)//long userId, string applicationid, string role)
        {
            UserRole userrole = GetByUserID(userRole.UserID);
            if (userrole == null)
                throw new ServiceException(ClientSideErrors.INVALID_USER_ID);
            if (userrole.RoleID != userRole.ID)
            {
                userrole.RoleID = userRole.RoleID;
                userrole.UpdatedBy = userRole.UpdatedBy;
                Update(userrole);
            }
            return userrole;
        }

        public UserRole GetByUserID(long userId)
        {
            return GetItems(commandType: CommandType.Text,
                   $"select * from {TableName} ur where ur.UserID=@userid",
                   new { userid = userId }).FirstOrDefault();
        }

        private bool IsValidRole(string applicationCode, string roleCode)
        {
            string sql = "select 1 from [Application] a inner join [Role] r on a.Id = R.ApplicationID where a.Code=@applicationCode and r.Code=@role";
            return DatabaseSession.Session.Query<int>(sql, new { applicationCode = applicationCode, role = roleCode }, commandType: CommandType.Text).FirstOrDefault() == 1;
        }
    }
}