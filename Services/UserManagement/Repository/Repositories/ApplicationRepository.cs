using Common.DataAccess;
using Common.DataAccess.Interfaces;
using Dapper;
using System.Data;
using System.Linq;
using UserManagement.Repository.Interfaces;
using UserManagement.Repository.Models;

namespace UserManagement.Repository.Repositories
{
    public class ApplicationRepository : DapperRepositoryBase<Application>, IApplicationRepository
    {
        public ApplicationRepository(IDatabaseSession databaseSession) : base(databaseSession)
        {
        }

        public long GetID(string applicationCode, IDbTransaction trans = null)
        {
            string sql = "select a.ID from [Application] a where a.Code=@applicationCode";
            return DatabaseSession.Session.Query<long>(sql, new { applicationCode = applicationCode }, commandType: CommandType.Text, transaction: trans).FirstOrDefault();
        }
    }
}
