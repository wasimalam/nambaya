using Common.DataAccess.Interfaces;
using System.Collections.Generic;

namespace Cardiologist.Repository.Interfaces
{
    public interface ICardiologistRepository : IDapperRepositoryBase<Models.Cardiologist>
    {
        Models.Cardiologist GetByEmail(string email);
        IEnumerable<Models.Cardiologist> GetAll();
        long GetCount();
    }
}