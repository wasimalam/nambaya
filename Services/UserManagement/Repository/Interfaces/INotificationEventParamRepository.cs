using Common.DataAccess.Interfaces;
using System.Collections.Generic;
using UserManagement.Repository.Models;

namespace UserManagement.Repository.Interfaces
{
    public interface INotificationEventParamRepository : IDapperRepositoryBase<NotificationEventParam>
    {
        IEnumerable<NotificationEventParam> GetByEventType(long eventtypeid);
    }
}
