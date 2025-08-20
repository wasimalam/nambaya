using Common.DataAccess;
using Common.DataAccess.Interfaces;
using UserManagement.Repository.Interfaces;
using UserManagement.Repository.Models;

namespace UserManagement.Repository.Repositories
{
    public class CredentialHistoryRepository : DapperRepositoryBase<CredentialHistory>, ICredentialHistoryRepository
    {
        public CredentialHistoryRepository(IDatabaseSession connectionFactory) : base(connectionFactory)
        {
        }
    }
}
