using Cardiologist.Repository.Interfaces;
using Common.DataAccess;
using Common.DataAccess.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Cardiologist.Repository.Repositories
{
    public class NurseRepository : DapperRepositoryBase<Repository.Models.Nurse>, INurseRepository
    {
        public NurseRepository(IDatabaseSession session) : base(session)
        {
        }
        public Models.Nurse GetByEmail(string email)
        {
            string sql = $"Select * from {TableName} where email=@email";
            return GetItems(System.Data.CommandType.Text, sql, new { email = email }).FirstOrDefault();
        }
        public IEnumerable<Models.Nurse> GetAll()
        {
            return GetItems(System.Data.CommandType.Text, $"Select * from {TableName}");
        }

        public long GetCount()
        {
            return ExecuteScalar<long>(System.Data.CommandType.Text, $"Select count(id) from {TableName}");
        }
    }
}