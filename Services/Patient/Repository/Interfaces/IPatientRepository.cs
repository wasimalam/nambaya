using Common.DataAccess.Interfaces;
using Common.Infrastructure;

namespace Patient.Repository.Interfaces
{
    public interface IPatientRepository : IDapperRepositoryBase<Models.Patient>
    {
        Models.Patient GetByPharmacyPatientID(string pharmacypatientid);
        Models.Patient GetByPatientCaseID(long patientcaseid);
        new PagedResults<Models.Patient> GetAllPages(int limit, int offset, string orderBy = null, IFilter parameters = null);
        PagedResults<Models.Patient> GetAllPatientCasePages(int limit, int offset, string orderBy = null, IFilter parameters = null);
        Models.Patient GetByAssignedDeviceId(long deviceid);
        void Delete(long patientid);
    }
}
