using Common.DataAccess.Interfaces;

namespace Patient.Repository.Interfaces
{
    public interface IDoctorRepository : IDapperRepositoryBase<Models.Doctor>
    {
        Models.Doctor GetByDoctorID(string doctorid);
    }
}
