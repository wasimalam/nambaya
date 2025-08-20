using Common.BusinessObjects;

namespace NambayaUser.Contracts.Interfaces
{
    public interface IUserRegistrationNotificationService
    {
        void NotifyRegitration(BaseUserBO message);
    }
}
