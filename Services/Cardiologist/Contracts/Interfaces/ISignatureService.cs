using Cardiologist.Contracts.Models;
using Patient.Contracts.Models;

namespace Cardiologist.Contracts.Interfaces
{
    public interface ISignatureService
    {
        SignaturesBO GetSignatures();
        string GenerateSignatureSaveOTP();
        SignaturesBO VerifySignatureSave(UpdateSignatureOtpRequest req);
        string GenerateSignatureDeleteOTP();
        void VerifySignatureDelete(UpdateSignatureOtpRequest req);
    }
}
