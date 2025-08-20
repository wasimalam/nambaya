using Common.DataAccess;
using Common.DataAccess.Interfaces;
using Pharmacist.Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Pharmacist.Repository.Repositories
{
    public class DeviceRepository : DapperRepositoryBase<Models.Device>, IDeviceRepository
    {
        public DeviceRepository(IDatabaseSession databaseSession) : base(databaseSession)
        {
        }
        public IEnumerable<Models.Device> GetDevices(long pharmacyid)
        {
            string sql = $"Select * from {TableName} where pharmacyid=@pharmacyid";
            return GetItems(System.Data.CommandType.Text, sql, new { pharmacyid = pharmacyid });
        }
        public Models.Device GetDeviceBySerial(string serialnumber, long pharmacyid)
        {
            string sql = $"Select * from {TableName} where pharmacyid=@pharmacyid and SerialNumber=@serialnumber";
            return GetItems(System.Data.CommandType.Text, sql, new { pharmacyid = pharmacyid , serialnumber = serialnumber}).FirstOrDefault();
        }
    }
}
