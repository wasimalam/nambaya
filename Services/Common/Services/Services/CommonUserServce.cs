using Common.BusinessObjects;
using Common.BusinessObjects.Interfaces;
using Common.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Common.Infrastructure.Exceptions;

namespace Common.Services.Services
{
    public class CommonUserServce : ICommonUserServce
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly WebServiceConfiguration _webApiConf;
        public CommonUserServce(IServiceProvider serviceProvider, WebServiceConfiguration webApiConf)
        {
            _serviceProvider = serviceProvider;
            _webApiConf = webApiConf;
        }
        public List<UserSettingBO> GetSettings(string loginname)
        {
            using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.UserManagementServiceBaseUrl))
            {
                var res = apiClient.InternalServiceGetAsync($"api/v1/user/settings/{loginname}").Result;
                return JsonSerializer.Deserialize<List<UserSettingBO>>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            }
        }
        public void UpdateSettings(string loginname, List<UserSettingBO> userSettings)
        {
            using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.UserManagementServiceBaseUrl))
            {
                var res = apiClient.InternalServicePostAsync($"api/v1/user/settings/{loginname}", JsonSerializer.Serialize(userSettings)).Result;
            }
        }

        public async Task ChangeCredentials(string loginName, ChangeCredentialBO userCredential)
        {
            using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.UserManagementServiceBaseUrl))
            {
                await apiClient.InternalServicePostAsync($"api/v1/user/ChangeCredentials/{loginName}", JsonSerializer.Serialize(userCredential));
            }
        }
    }
}