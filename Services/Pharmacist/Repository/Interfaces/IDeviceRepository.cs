using Common.DataAccess.Interfaces;
using System.Collections.Generic;

namespace Pharmacist.Repository.Interfaces
{
    public interface IDeviceRepository : IDapperRepositoryBase<Models.Device>
    {
        IEnumerable<Models.Device> GetDevices(long pharmacyid);
        Models.Device GetDeviceBySerial(string serialnumber, long pharmacyid);
    }
}