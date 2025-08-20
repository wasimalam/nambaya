using Common.BusinessObjects;
using Common.Infrastructure;
using System.Collections.Generic;
using UserManagement.Contracts.Models;

namespace UserManagement.Contracts.Interfaces
{
    public interface INotificationSetupService
    {
        IEnumerable<LookupsBO> GetNotificationEventTypes();
        IEnumerable<LookupsBO> GetNotificationTemplateTypes();
        PagedResults<NotificationTemplateBO> GetNotificationTemplates(int limit, int offset, string orderby, string param);
        NotificationTemplateBO GetNotificationTemplate(long id);
        long AddTemplate(NotificationTemplateBO notificationTemplateBO);
        void UpdateTemplate(NotificationTemplateBO notificationTemplateBO);
        IEnumerable<NotificationEventTypeBO> GetNotificationTypesByEvent(long eventid);
        IEnumerable<NotificationEventParamBO> GetNotificationParamsByEvent(long eventid);
        NotificationTemplateBO GetNotificationTemplateByEventTemplate(long eventtypeid, long templatetypeid);
    }
}
