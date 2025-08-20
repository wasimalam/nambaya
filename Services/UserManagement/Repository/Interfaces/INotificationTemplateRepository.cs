using Common.DataAccess.Interfaces;
using UserManagement.Repository.Models;

namespace UserManagement.Repository.Interfaces
{
    public interface INotificationTemplateRepository : IDapperRepositoryBase<NotificationTemplate>
    {
        NotificationTemplate GetNotificationTemplateByEventTemplate(long eventtypeid, long templatetypeid);
    }
}
