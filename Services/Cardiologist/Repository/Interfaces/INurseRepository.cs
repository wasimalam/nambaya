using Common.DataAccess.Interfaces;
using System.Collections.Generic;

namespace Cardiologist.Repository.Interfaces
{
    public interface INurseRepository : IDapperRepositoryBase<Models.Nurse>
    {
        Models.Nurse GetByEmail(string email);
        IEnumerable<Models.Nurse> GetAll();
        long GetCount();
    }
}