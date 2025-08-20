using CentralGroup.Repository.Interfaces;
using Common.DataAccess;
using Common.DataAccess.Interfaces;
using System.Linq;

namespace CentralGroup.Repository.Repositories
{
    public class CentralGroupRepository : DapperRepositoryBase<Repository.Models.CentralGroup>, ICentralGroupRepository
    {
        public CentralGroupRepository(IDatabaseSession session) : base(session)
        {
        }
        public Models.CentralGroup GetByEmail(string email)
        {
            string sql = $"Select * from {TableName} where email=@email";
            return GetItems(System.Data.CommandType.Text, sql, new { email = email }).FirstOrDefault();
        }
        public long GetCount()
        {
            return ExecuteScalar<long>(System.Data.CommandType.Text, $"Select count(id) from {TableName}");
        }
    }
}

