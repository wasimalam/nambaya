using Common.BusinessObjects.ConsumerMessages;
using Patient.Contracts.Models;

namespace Patient.Contracts.Interfaces
{
    public interface ICaseDispatchService
    {
        CaseDispatchDetailBO GetCaseDispatchDetails(long patientcaseid);
        long AddCaseDispatchDetails(CaseDispatchDetailBO caseDispatchDetailBO);
        void UpdateCaseDetail(PatientCaseStatusDetailMessage message);
    }
}
