using Patient.Contracts.Models;

namespace CentralGroup.Contracts.Interfaces
{
    public interface IDeactivatePatientEventService
    {
        void DeactivatePatienOTPNotify(PatientUserOtp userOtp);
    }  

}
