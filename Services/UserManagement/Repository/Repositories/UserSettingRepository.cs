using Common.DataAccess;
using Common.DataAccess.Interfaces;
using System.Collections.Generic;
using System.Data;
using UserManagement.Repository.Interfaces;
using UserManagement.Repository.Models;
using System.Linq;

namespace UserManagement.Repository.Repositories
{
    public class UserSettingRepository : DapperRepositoryBase<UserSetting>, IUserSettingRepository
    {
        public UserSettingRepository(IDatabaseSession databaseSession) : base(databaseSession)
        {
        }
        public IEnumerable<UserSetting> GetByUserID(long userId)
        {
            return GetItems(commandType: CommandType.Text,
                   $"select * from {TableName} ur where ur.UserID=@userid",
                   new { userid = userId });
        }
        public UserSetting GetByCode(long userId, string code)
        {
            return GetItems(commandType: CommandType.Text,
                  $"select * from {TableName} ur where ur.UserID=@userid and code = @code",
                  new { userid = userId , code = code}).FirstOrDefault();
        }
    }
}