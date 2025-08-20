using Common.Infrastructure;
using Logging.Contracts.Models;

namespace Logging.Contracts.Interfaces
{
    public interface INotificationsLogService
    {
        PagedResults<NotificationsBO> GetLogs(int limit, int offset, string orderby, string param);
        void ExecuteInsert(NotificationsBO logObj);
    }
}
