using Patient.Contracts.Models;

namespace Patient.Contracts.Interfaces
{
    public interface IAdditionalInfoService
    {
        PatientAdditionalInfoBO GetPatientAdditionalInfoByCaseID(long patientcaseid);
        long AddPatientAdditionalInfo(PatientAdditionalInfoBO patientAdditionalInfoBO);
        void UpdatePatientAdditionalInfo(PatientAdditionalInfoBO patientAdditionalInfoBO);
    }
}
