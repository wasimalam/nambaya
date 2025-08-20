using Common.Infrastructure;
using Patient.Contracts.Models;

namespace Patient.Contracts.Interfaces
{
    public interface IDoctorService
    {
        PagedResults<DoctorBO> GetDoctors(int limit, int offset, string orderby, string param);
        DoctorBO GetDoctorById(long id);
        long AddDoctor(DoctorBO cardiologist);
        void UpdateDoctor(DoctorBO cardiologist);
        void DeleteDoctor(DoctorBO cardiologist);
    }
}
