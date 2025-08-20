using Common.DataAccess;
using Common.DataAccess.Interfaces;
using Logging.Repository.Interfaces;

namespace Logging.Repository.Repositories
{
    public class NotificationsRepository : DapperRepositoryBase<Repository.Models.Notifications>, INotificationsRepository
    {
        public NotificationsRepository(IDatabaseSession session) : base(session)
        {
        }
    }
}