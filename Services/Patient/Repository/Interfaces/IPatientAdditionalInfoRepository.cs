using Common.DataAccess.Interfaces;
using Patient.Repository.Models;

namespace Patient.Repository.Interfaces
{
    public interface IPatientAdditionalInfoRepository : IDapperRepositoryBase<PatientAdditionalInfo>
    {
        PatientAdditionalInfo GetByPatientCaseId(long patientcaseid);
    }
}
