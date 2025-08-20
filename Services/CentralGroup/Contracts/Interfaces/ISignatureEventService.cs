using Common.Infrastructure;
using Patient.Contracts.Models;

namespace CentralGroup.Contracts.Interfaces
{
    public interface ISignatureSaveEventService
    {
        void SignatureSaveOTPNotify(UserOtp userOtp);
    }
    public interface ISignatureDeleteEventService
    {
        void SignatureDeleteOTPNotify(UserOtp userOtp);
    }
    public interface IDESignatureEventService
    {
        void DESignatureOTPNotify(PatientUserOtp userOtp);
    }

}
