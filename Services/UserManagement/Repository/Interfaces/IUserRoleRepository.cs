using Common.DataAccess.Interfaces;
using UserManagement.Repository.Models;

namespace UserManagement.Repository.Interfaces
{
    public interface IUserRoleRepository : IDapperRepositoryBase<UserRole>
    {
        UserRole UpdateUserRole(UserRole userRole);
        UserRole GetByUserID(long userId);
    }
}