using Common.DataAccess.Interfaces;
using UserManagement.Repository.Models;

namespace UserManagement.Repository.Interfaces
{
    public interface ICredentialRepository : IDapperRepositoryBase<Credential>
    {
        void InsertPassword(Credential credential);
        bool IsValid(long userId, string password);
        new Credential GetByID(long userId);
        new void Update(Credential obj);
    }
}