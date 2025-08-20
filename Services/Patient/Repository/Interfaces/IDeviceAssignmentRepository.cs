using Common.DataAccess.Interfaces;

namespace Patient.Repository.Interfaces
{
    public interface IDeviceAssignmentRepository : IDapperRepositoryBase<Models.DeviceAssignment>
    {
        Models.DeviceAssignment Get(long deviceID, bool isAssigned);
        Models.DeviceAssignment GetByCaseId(long patientcaseid);
    }
}