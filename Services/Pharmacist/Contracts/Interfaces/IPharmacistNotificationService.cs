using Patient.Contracts.Models;

namespace Pharmacist.Contracts.Interfaces
{
    public interface IPharmacistNotificationService
    {
        void NotifyQuickEvaluation(PatientBO patient);
        void RemindChargingDevice();
    }
}
