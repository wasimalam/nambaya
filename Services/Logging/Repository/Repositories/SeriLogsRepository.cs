using Common.DataAccess;
using Common.DataAccess.Interfaces;
using Logging.Repository.Interfaces;
using System.Collections.Generic;

namespace Logging.Repository.Repositories
{
    public class SeriLogsRepository : DapperRepositoryBase<Repository.Models.SeriLogs>, ISerilogsRepository
    {
        public SeriLogsRepository(IDatabaseSession session) : base(session)
        {
        }
        public IEnumerable<Models.SeriLogs> GetByRequestId(string requestid)
        {
            return GetItems(System.Data.CommandType.Text, $"Select * from {TableName} where requestid=@requestid",
                new { requestid = requestid });
        }
    }
}