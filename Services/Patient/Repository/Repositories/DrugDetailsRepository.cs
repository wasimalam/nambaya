using Common.DataAccess;
using Common.DataAccess.Interfaces;
using Patient.Repository.Interfaces;
using Patient.Repository.Models;
using System.Collections.Generic;

namespace Patient.Repository.Repositories
{
    public class DrugDetailsRepository : DapperRepositoryBase<DrugDetails>, IDrugDetailsRepository
    {
        public DrugDetailsRepository(IDatabaseSession session) : base(session)
        {

        }
        public IEnumerable<DrugDetails> GetByDrugGroupId(long druggroupid)
        {
            return GetItems(System.Data.CommandType.Text, $"select * from {TableName} where druggroupid = @druggroupid ", new { druggroupid = druggroupid });
        }
        public void DeleteByGroupId(long druggroupid)
        {
            Execute(System.Data.CommandType.Text, $"delete from {TableName} where druggroupid = @druggroupid ", new { druggroupid = druggroupid });
        }
    }
}