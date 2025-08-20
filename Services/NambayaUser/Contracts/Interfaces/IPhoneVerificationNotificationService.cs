using Common.Infrastructure;

namespace NambayaUser.Contracts.Interfaces
{
    public interface IPhoneVerificationNotificationService
    {
        void NotifyPhoneVerification(UserOtp user);
    }
}
