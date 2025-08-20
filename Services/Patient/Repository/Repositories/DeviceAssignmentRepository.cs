using Common.DataAccess;
using Common.DataAccess.Interfaces;
using Patient.Repository.Interfaces;
using Patient.Repository.Models;
using System.Linq;

namespace Patient.Repository.Repositories
{
    public class DeviceAssignmentRepository : DapperRepositoryBase<DeviceAssignment>, IDeviceAssignmentRepository
    {
        public DeviceAssignmentRepository(IDatabaseSession session) : base(session)
        {
        }

        public DeviceAssignment Get(long deviceID, bool isAssigned)
        {
            return GetItems(System.Data.CommandType.Text, $"select * from {TableName} where deviceID=@deviceID and isAssigned=@isAssigned order by 1 desc ",
                new { deviceID = deviceID, isAssigned = isAssigned }).FirstOrDefault();
        }

        public DeviceAssignment GetByCaseId(long patientcaseid)
        {
            return GetItems(System.Data.CommandType.Text, $"select * from {TableName} where patientcaseid=@patientcaseid  order by 1 desc ",
                new { patientcaseid = patientcaseid }).FirstOrDefault();
        }
    }
}
