using Common.Infrastructure;
using Pharmacist.Contracts.Models;

namespace Pharmacist.Contracts.Interfaces
{
    public interface IPharmacyService
    {
        PagedResults<PharmacyBO> GetPharmacies(int limit, int offset, string orderby, string param);
        PharmacyBO GetPharmacyById(long id);
        string GetPharmacyIdentification();
        PharmacyBO GetPharmacyByEmail(string email);
        PharmacyBO GetPharmacyByPharmacist(string pharmacistEmail);
        long AddPharmacy(PharmacyBO pharmacy);
        void UpdatePharmacy(PharmacyBO pharmacy);
        void DeletePharmacy(PharmacyBO pharmacy);
        long GetTotal();
        string GeneratePhoneVerification(VerifyPhoneRequest req);
        PharmacyBO VerifyPhoneVerification(VerifyPhoneOtpRequest req);
        PagedResults<PharmacyBO> GetAllPharamacists();
    }
}