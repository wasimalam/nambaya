using Patient.Contracts.Models;
using UserManagement.Contracts.Models;

namespace CentralGroup.Contracts.Interfaces
{
    public interface IDECompletedEventNotificationService
    {
        void Notify(DECompletedEventPayloadBO dECompletedEventPayloadBO);
        NotificationTemplateBO GetEmailNotificationTemplate(long eventtypeid);
    }
}
