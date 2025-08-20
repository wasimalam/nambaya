using Common.DataAccess.Interfaces;

namespace CentralGroup.Repository.Interfaces
{
    public interface ICentralGroupRepository : IDapperRepositoryBase<Models.CentralGroup>
    {
        Models.CentralGroup GetByEmail(string email);
        long GetCount();
    }
}
