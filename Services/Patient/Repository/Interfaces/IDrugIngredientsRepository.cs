using Common.DataAccess.Interfaces;
using Patient.Repository.Models;
using System.Collections.Generic;

namespace Patient.Repository.Interfaces
{
    public interface IDrugIngredientsRepository : IDapperRepositoryBase<DrugIngredients>
    {
        IEnumerable<DrugIngredients> GetByDrugDetailsID(long drugdetailsid);
        void DeleteByDrugDetailsID(long drugdetailsid);
        void DeleteByGroupId(long druggroupid);
    }
}
