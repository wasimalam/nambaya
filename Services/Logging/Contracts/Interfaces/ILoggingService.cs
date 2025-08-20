using Common.Infrastructure;
using Logging.Contracts.Models;

namespace Logging.Contracts.Interfaces
{
    public interface ILoggingService
    {
        PagedResults<SeriLogsBO> GetLogs(int limit, int offset, string orderby, string param);
    }
}
