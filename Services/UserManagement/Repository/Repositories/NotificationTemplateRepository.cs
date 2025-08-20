using Common.DataAccess;
using Common.DataAccess.Interfaces;
using System.Data;
using System.Linq;
using UserManagement.Repository.Interfaces;
using UserManagement.Repository.Models;

namespace UserManagement.Repository.Repositories
{
    public class NotificationTemplateRepository : DapperRepositoryBase<NotificationTemplate>, INotificationTemplateRepository
    {
        public NotificationTemplateRepository(IDatabaseSession databaseSession) : base(databaseSession)
        {

        }

        public NotificationTemplate GetNotificationTemplateByEventTemplate(long eventtypeid, long templatetypeid)
        {
            return GetItems(commandType: CommandType.Text, $"select * from {TableName} where EventTypeID=@eventtypeid and templatetypeid=@templatetypeid", 
                new { eventtypeid = eventtypeid, templatetypeid = templatetypeid }).FirstOrDefault();
        }
    }
}
