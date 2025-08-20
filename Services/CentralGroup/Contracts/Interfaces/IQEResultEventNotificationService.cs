using Patient.Contracts.Models;

namespace CentralGroup.Contracts.Interfaces
{
    public interface IQEResultEventNotificationService
    {
        void Notify(QEResultEventPayloadBO patient);
    }
}
