using Cardiologist.Contracts.Models;
using Common.Infrastructure;

namespace Cardiologist.Contracts.Interfaces
{
    public interface ICardiologistService
    {
        PagedResults<CardiologistBO> GetCardiologists(int limit, int offset, string orderby, string param);
        CardiologistBO GetCardiologistById(long id);
        CardiologistBO GetCardiologistByEmail(string email);
        long AddCardiologist(CardiologistBO cardiologist);
        void UpdateCardiologist(CardiologistBO cardiologist);
        void DeleteCardiologist(CardiologistBO cardiologist);
        long GetTotal();
        string GeneratePhoneVerification(VerifyPhoneRequest req);
        CardiologistBO VerifyPhoneVerification(VerifyPhoneOtpRequest req);
    }
}
