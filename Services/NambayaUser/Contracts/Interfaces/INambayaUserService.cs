using Common.Infrastructure;
using NambayaUser.Contracts.Models;

namespace NambayaUser.Contracts.Interfaces
{
    public interface INambayaUserService
    {
        PagedResults<UserBO> GetUsers(int limit, int offset, string orderby, string param);
        UserBO GetUserById(long id);
        UserBO GetUserByEmail(string email);
        long AddUser(UserBO userBO);
        void UpdateUser(UserBO userBO);
        void DeleteUser(UserBO userBO);
        object GetRoles(string applicationCode);
        long GetTotal();
        object GetStats();
        string GeneratePhoneVerification(VerifyPhoneRequest req);
        UserBO VerifyPhoneVerification(VerifyPhoneOtpRequest req);

    }
}
