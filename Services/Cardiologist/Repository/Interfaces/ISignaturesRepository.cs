using Common.DataAccess.Interfaces;

namespace Cardiologist.Repository.Interfaces
{
    public interface ISignaturesRepository : IDapperRepositoryBase<Models.Signatures>
    {
        Models.Signatures GetByCardiologist(long cardiologistId);
    }
}