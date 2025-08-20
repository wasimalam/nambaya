using CentralGroup.Contracts.Models;
using Common.Infrastructure;

namespace CentralGroup.Contracts.Interfaces
{
    public interface ICentralGroupService
    {
        PagedResults<CentralGroupBO> GetCentralGroup(int limit, int offset, string orderby, string param);
        CentralGroupBO GetCentralGroupById(long id);
        CentralGroupBO GetCentralGroupByEmail(string email);
        long AddCentralGroup(CentralGroupBO centralGroup);
        void UpdateCentralGroup(CentralGroupBO centralGroup);
        void DeleteCentralGroup(CentralGroupBO centralGroup);
        long GetTotal();
        string GeneratePhoneVerification(VerifyPhoneRequest req);
        CentralGroupBO VerifyPhoneVerification(VerifyPhoneOtpRequest req);
    }
}
