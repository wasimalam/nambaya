using Common.DataAccess;
using Common.DataAccess.Interfaces;
using System.Collections.Generic;
using System.Data;
using UserManagement.Repository.Interfaces;
using UserManagement.Repository.Models;

namespace UserManagement.Repository.Repositories
{
    public class NotificationEventParamRepository : DapperRepositoryBase<NotificationEventParam>, INotificationEventParamRepository
    {
        public NotificationEventParamRepository(IDatabaseSession databaseSession) : base(databaseSession)
        {

        }
        public IEnumerable<NotificationEventParam> GetByEventType(long eventtypeid)
        {
            return GetItems(commandType: CommandType.Text, $"select * from {TableName} where EventTypeID=@eventtypeid", new { eventtypeid = eventtypeid });
        }
    }
}
