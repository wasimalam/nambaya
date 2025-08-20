using Common.DataAccess.Interfaces;
using Patient.Repository.Models;
using System.Collections.Generic;

namespace Patient.Repository.Interfaces
{
    public interface IDrugFreeTextRepository : IDapperRepositoryBase<DrugFreeText>
    {
        IEnumerable<DrugFreeText> GetByDrugGroupId(long druggroupid);
        void DeleteByGroupId(long druggroupid);
    }
}
