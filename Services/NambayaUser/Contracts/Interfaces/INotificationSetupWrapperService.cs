using Common.BusinessObjects;
using Common.Infrastructure;
using System.Collections.Generic;
using UserManagement.Contracts.Models;

namespace NambayaUser.Contracts.Interfaces
{
    public interface INotificationSetupWrapperService
    {
        IEnumerable<LookupsBO> GetNotificationEventTypes();
        IEnumerable<LookupsBO> GetNotificationTemplateTypes();
        PagedResults<NotificationTemplateBO> GetNotificationTemplates(int limit, int offset, string orderby, string param);
        NotificationTemplateBO GetNotificationTemplate(long id);
        NotificationTemplateBO GetNotificationTemplate(long eventtypeid, long templatetypeid);
        NotificationTemplateBO AddTemplate(NotificationTemplateBO notificationTemplateBO);
        NotificationTemplateBO UpdateTemplate(NotificationTemplateBO notificationTemplateBO);
        IEnumerable<NotificationEventTypeBO> GetNotificationTypesByEvent(long eventid);
        IEnumerable<NotificationEventParamBO> GetNotificationParamsByEvent(long eventid);
    }
}
