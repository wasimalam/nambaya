using Common.DataAccess;
using Common.DataAccess.Interfaces;
using Patient.Repository.Models;
using System.Collections.Generic;

namespace Patient.Repository.Interfaces
{
    public class DrugIngredientsRepository : DapperRepositoryBase<DrugIngredients>, IDrugIngredientsRepository
    {
        public DrugIngredientsRepository(IDatabaseSession session) : base(session)
        {
        }
        public IEnumerable<DrugIngredients> GetByDrugDetailsID(long drugdetailsid) 
        {
            return GetItems(System.Data.CommandType.Text, $"select * from {TableName} where drugdetailsid = @drugdetailsid ", new { drugdetailsid = drugdetailsid });
        }
        public void DeleteByDrugDetailsID(long drugdetailsid)
        {
            Execute(System.Data.CommandType.Text, $"delete from {TableName} where drugdetailsid = @drugdetailsid ", new { drugdetailsid = drugdetailsid });
        }

        public void DeleteByGroupId(long druggroupid)
        {
            Execute(System.Data.CommandType.Text, $"delete w from  {TableName} w INNER JOIN drugdetails d on w.drugdetailsid=d.id where d.druggroupid = @druggroupid ", new { druggroupid = druggroupid });
        }
    }
}
