using Patient.Contracts.Models;

namespace Pharmacist.Contracts.Interfaces
{
    public interface IDeviceReminderService
    {
        void RemindOfDevice(DeviceReminderPayLoadBO deviceReminderPayLoad);
    }
}
