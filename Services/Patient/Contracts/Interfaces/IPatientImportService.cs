using Patient.Contracts.Models;

namespace Patient.Contracts.Interfaces
{
    public interface IPatientImportService
    {
        PatientBO ImportXml(byte[] fileContent, string pharmacypatientid, string patientEmail, string patientPhone, string fileName, string fileContentType);
    }
}
