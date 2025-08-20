using Common.BusinessObjects;
using Common.Infrastructure;
using Common.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Navigator.Contracts.Interfaces;
using Navigator.Contracts.Models;
using Newtonsoft.Json;
using System;
using System.Threading;

namespace Navigator.Service
{
    public class PharmacyService : BaseService, IPharmacyService
    {
        private readonly ILogger<PatientService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly WebServiceConfiguration _webApiConf;
        public PharmacyService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<PatientService>>();
            _serviceProvider = serviceProvider;
            _webApiConf = serviceProvider.GetRequiredService<WebServiceConfiguration>();
        }

        public void Import(PharmacyImportBO pharmacyImportBO)
        {

            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PharmacistServiceBaseUrl, TimeSpan.FromMinutes(30)))
                {
                    var sessionId = Guid.NewGuid().ToString();
                    Thread.SetData(Thread.GetNamedDataSlot(LoggingConstants.CorrelationId), sessionId);
                    Thread.SetData(Thread.GetNamedDataSlot("NambayaSession"), new SessionContext { LoginName = "Import" , RoleCode=RoleCodes.NambayaUser});
                    _logger.LogInformation($"Starting Import Pharmacy {pharmacyImportBO.Email}  {pharmacyImportBO.Pharmaca}");
                    var res = apiClient.InternalServicePostAsync($"api/v1/pharmacy", JsonConvert.SerializeObject(
                        new
                        {
                            Name = pharmacyImportBO.Pharmaca,
                            Email = pharmacyImportBO.Email,
                            pharmacyImportBO.Phone,
                            pharmacyImportBO.ZipCode,
                            Address = pharmacyImportBO.Address + ' ' + pharmacyImportBO.City,
                            pharmacyImportBO.Contact,
                            pharmacyImportBO.Identification,
                            pharmacyImportBO.Fax,
                            Role = RoleCodes.Pharmacy,
                            IsActive = true
                        })).Result;

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Import failed for {pharmacyImportBO.Pharmaca} , {pharmacyImportBO.Identification}");
            }
        }
    }
}
