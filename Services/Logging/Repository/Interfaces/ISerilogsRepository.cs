using Common.DataAccess.Interfaces;
using System.Collections.Generic;

namespace Logging.Repository.Interfaces
{
    public interface ISerilogsRepository : IDapperRepositoryBase<Models.SeriLogs>
    {
        IEnumerable<Models.SeriLogs> GetByRequestId(string requestid);
    }
}