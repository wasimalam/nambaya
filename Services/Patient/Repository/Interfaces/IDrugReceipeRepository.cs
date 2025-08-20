using Common.DataAccess.Interfaces;
using Patient.Repository.Models;
using System.Collections.Generic;

namespace Patient.Repository.Interfaces
{
    public interface IDrugReceipeRepository : IDapperRepositoryBase<DrugReceipe>
    {
        IEnumerable<DrugReceipe> GetByDrugGroupId(long druggroupid);
        void DeleteByGroupId(long druggroupid);
    }
}
