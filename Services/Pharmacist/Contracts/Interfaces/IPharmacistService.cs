using Common.Infrastructure;
using Pharmacist.Contracts.Models;

namespace Pharmacist.Contracts.Interfaces
{
    public interface IPharmacistService
    {
        PagedResults<PharmacistBO> GetPharmacists(int limit, int offset, string orderby, string param);
        PharmacistBO GetPharmacistById(long id);
        PharmacistBO GetPharmacistByEmail(string email);
        long AddPharmacist(PharmacistBO pharmacy);
        void UpdatePharmacist(PharmacistBO pharmacy);
        void DeletePharmacist(PharmacistBO pharmacy);
        string GeneratePhoneVerification(VerifyPhoneRequest req);
        PharmacistBO VerifyPhoneVerification(VerifyPhoneOtpRequest req);
        PagedResults<PharmacistBO> GetAllPharamacists();
    }
}
