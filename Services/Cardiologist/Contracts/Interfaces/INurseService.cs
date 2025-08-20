using System.Collections.Generic;
using Cardiologist.Contracts.Models;
using Common.Infrastructure;

namespace Cardiologist.Contracts.Interfaces
{
    public interface INurseService
    {
        PagedResults<NurseBO> GetNurses(int limit, int offset, string orderby, string param);
        PagedResults<NurseBO> GetAllNurses(int qLimit, int offset, string qOrderby, string param);
        NurseBO GetNurseById(long id);
        NurseBO GetNurseByEmail(string email);
        long AddNurse(NurseBO nurse);
        void UpdateNurse(NurseBO nurse);
        void DeleteNurse(NurseBO nurse);
        long GetTotal();
        string GeneratePhoneVerification(VerifyPhoneRequest req);
        NurseBO VerifyPhoneVerification(VerifyPhoneOtpRequest req);
    }
}
