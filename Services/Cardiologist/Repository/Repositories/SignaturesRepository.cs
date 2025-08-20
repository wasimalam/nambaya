using Cardiologist.Repository.Interfaces;
using Common.DataAccess;
using Common.DataAccess.Interfaces;
using System.Linq;

namespace Cardiologist.Repository.Repositories
{
    public class SignaturesRepository : DapperRepositoryBase<Repository.Models.Signatures>, ISignaturesRepository
    {
        public SignaturesRepository(IDatabaseSession session) : base(session)
        {
        }
        public Models.Signatures GetByCardiologist(long cardiologistId)
        {
            string sql = $"Select * from {TableName} where cardiologistId=@cardiologistId";
            return GetItems(System.Data.CommandType.Text, sql, new { cardiologistId }).FirstOrDefault();
        }
    }
}