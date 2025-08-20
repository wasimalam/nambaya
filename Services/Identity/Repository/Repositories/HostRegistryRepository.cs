using Common.DataAccess;
using Common.DataAccess.Interfaces;
using Identity.Repository.Interfaces;
using Identity.Repository.Models;
using System.Linq;

namespace Identity.Repository.Repositories
{
    public class HostRegistryRepository : DapperRepositoryBase<HostsResgistry>, IHostRegistryRepository
    {
        public HostRegistryRepository(IDatabaseSession databaseSession) : base(databaseSession)
        {
        }
        public string GetErrorUrl() => GetItems(System.Data.CommandType.Text, $"Select * from HostsRegistry where host='ErrorUrl'").ToList().FirstOrDefault()?.Address;

        public string GetLogInUrl()
        {
            return GetItems(System.Data.CommandType.Text, $"Select * from HostsRegistry where host='LoginUrl'").ToList().FirstOrDefault()?.Address;
        }

        public string GetLogOutUrl()
        {
            return GetItems(System.Data.CommandType.Text, $"Select * from HostsRegistry where host='LogOutUrl'").ToList().FirstOrDefault()?.Address;
        }
        public string GetIdentityServerUrl()
        {
            return GetItems(System.Data.CommandType.Text, $"Select * from HostsRegistry where host='IdentityServerUrl'").ToList().FirstOrDefault()?.Address;
        }
        public string[] GetCorsUrls()
        {
            return GetItems(System.Data.CommandType.Text, $"Select * from HostsRegistry where host='AllowedCorsUrl'").ToList().Select(p => p.Address).ToArray();
        }
    }
}
