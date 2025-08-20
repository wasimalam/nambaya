using Common.Infrastructure;
using Pharmacist.Contracts.Models;

namespace Pharmacist.Contracts.Interfaces
{
    public interface IDeviceService
    {
        PagedResults<DeviceBO> GetDevices(int limit, int offset, string orderby, string param);
        PagedResults<DeviceBO> GetDevicesByPharmacy(int limit, int offset, string orderby, string filter);
        DeviceBO GetDeviceById(long id);
        long AddDevice(DeviceBO device);
        void UpdateDevice(DeviceBO device);
        void DeleteDevice(DeviceBO device);
        DeviceBO AssignDevice(DeviceAssignmentBO deviceAssignment);
        DeviceAssignmentBO GetDeviceAssignmentByID(long devAssignmentId);
        DeviceAssignmentBO GetDeviceAssignmentByPatientCaseID(long patientcaseid);
        DeviceBO GetDeviceBySerial(string serialnumber);
    }
}
