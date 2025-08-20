using Patient.Contracts.Models;

namespace Navigator.Contracts.Interfaces
{
    public interface IPatientService
    {
        void Execute(EdfFileUpdatePayLoadBO patientBOt, int retryCount);
    }
}
