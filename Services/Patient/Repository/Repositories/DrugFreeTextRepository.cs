using Common.DataAccess;
using Common.DataAccess.Interfaces;
using Patient.Repository.Models;
using System.Collections.Generic;

namespace Patient.Repository.Interfaces
{
    public class DrugFreeTextRepository : DapperRepositoryBase<DrugFreeText>, IDrugFreeTextRepository
    {
        public DrugFreeTextRepository(IDatabaseSession session) : base(session)
        {
        }
        public IEnumerable<DrugFreeText> GetByDrugGroupId(long druggroupid)
        {
            return GetItems(System.Data.CommandType.Text, $"select * from {TableName} where druggroupid = @druggroupid ", new { druggroupid = druggroupid });
        }
        public void DeleteByGroupId(long druggroupid)
        {
            Execute(System.Data.CommandType.Text, $"delete from {TableName} where druggroupid = @druggroupid ", new { druggroupid = druggroupid });
        }
    }
}
