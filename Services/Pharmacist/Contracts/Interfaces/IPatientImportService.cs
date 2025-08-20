using Pharmacist.Contracts.Models;

namespace Pharmacist.Contracts.Interfaces
{
    public interface IPatientImportService
    {
        ImportStatus ImportPatient(string clientId, string secret, PharmacyBO pharmacyBO, string caseNumber, string patientEmail, string patientPhone, byte[] fileContent);
    }
}
