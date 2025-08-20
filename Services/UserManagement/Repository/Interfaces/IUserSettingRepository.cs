using Common.DataAccess.Interfaces;
using System.Collections.Generic;
using UserManagement.Repository.Models;

namespace UserManagement.Repository.Interfaces
{
    public interface IUserSettingRepository : IDapperRepositoryBase<UserSetting>
    {
        IEnumerable<UserSetting> GetByUserID(long userId);
        UserSetting GetByCode(long userId, string code);
    }
}