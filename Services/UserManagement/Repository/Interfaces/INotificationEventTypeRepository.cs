using Common.DataAccess.Interfaces;
using System.Collections.Generic;
using UserManagement.Repository.Models;

namespace UserManagement.Repository.Interfaces
{
    public interface INotificationEventTypeRepository : IDapperRepositoryBase<NotificationEventType>
    {
        IEnumerable<NotificationEventType> GetByEventType(long eventtypeid);
    }
}
