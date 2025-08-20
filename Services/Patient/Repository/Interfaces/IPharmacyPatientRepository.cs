using Common.DataAccess.Interfaces;

namespace Patient.Repository.Interfaces
{
    public interface IPharmacyPatientRepository : IDapperRepositoryBase<Models.PharmacyPatient>
    {
        Models.PharmacyPatient GetByPharmacyPatientID(string pharmacyPatientID);
    }
}
