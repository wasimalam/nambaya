using Common.BusinessObjects;
using Common.Infrastructure;
using Common.Infrastructure.Exceptions;
using Common.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Patient.Contracts.Models;
using Pharmacist.Contracts.Interfaces;
using Pharmacist.Contracts.Models;
using Pharmacist.Repository.Interfaces;
using System;
using System.Text.Json;

namespace Pharmacist.Service
{
    public class PatientImportService : BaseService, IPatientImportService
    {
        private readonly WebServiceConfiguration _webApiConf;
        private readonly IPharmacistRepository _pharmacistRepository;
        private readonly IPharmacyRepository _pharmacyRepository;
        private readonly IPharmacyService _pharmacyService;
        private readonly ILogger<PatientImportService> _logger;
        public PatientImportService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _webApiConf = serviceProvider.GetRequiredService<WebServiceConfiguration>();
            _pharmacistRepository = serviceProvider.GetRequiredService<IPharmacistRepository>();
            _pharmacyRepository = serviceProvider.GetRequiredService<IPharmacyRepository>();
            _pharmacyService = serviceProvider.GetRequiredService<IPharmacyService>();
            _logger = serviceProvider.GetRequiredService<ILogger<PatientImportService>>();
        }

        public ImportStatus ImportPatient(string clientId, string secret, PharmacyBO pharmacyBO, string caseNumber, string patientEmail, string patientPhone, byte[] fileContent)
        {
            ImportStatus importStatus = new ImportStatus();
            string accessToken = "";
            _logger.LogInformation($"ImportPatient: caseNumber {caseNumber} patientEmail {patientEmail} patientPhone {patientPhone}");
            try
            {
                _logger.LogInformation("Checking for client auth");
                // first check client auth
                var apiSecretConf = new ApiRequestConfiguration() { Clientid = clientId, Secret = secret, Scope = "patient_import", ApiName = clientId };
                accessToken = ApiClient.GetAccessToken(_webApiConf.IdentityServerBaseUrl, apiSecretConf);

                // check for pharmacy exist if not create it
                var ph = _pharmacyRepository.GetByIdentification(pharmacyBO.Identification);
                if (ph == null)
                {
                    _logger.LogInformation("Pharmacy not exist adding new pharmacy");
                    pharmacyBO.IsActive = true;
                    pharmacyBO.IsLocked = false;
                    pharmacyBO.CreatedBy = clientId;
                    pharmacyBO.Role = RoleCodes.Pharmacy;
                    long phid = _pharmacyService.AddPharmacy(pharmacyBO);
                    ph = _pharmacyRepository.GetByID(phid);
                    importStatus.PharmacyStatus = "PHARMACY_CREATED";
                }
                else
                    importStatus.PharmacyStatus = "PHARMACY_ALREADY_EXISTS";

                if (ph == null)
                    throw new ServiceException("FAILED_TO_CREATE_PHARMACY");
                //import patient data by calling 
                var sessionContext = new SessionContext() { LoginName = ph.Email, PharmacyID = ph.ID, RoleCode = RoleCodes.Pharmacy };
                var pharmacyPatientId = $"{pharmacyBO.Identification}-{caseNumber}";
                ImportXml(fileContent, pharmacyPatientId, patientEmail, patientPhone, $"{pharmacyPatientId}.zip", "application/zip", sessionContext, accessToken);
                importStatus.PatientStatus = "PATIENT_IMPORTED";
            }
            catch (Exception ex)
            {
                if (string.IsNullOrWhiteSpace(accessToken))
                    throw new ServiceException("INVALID_CLIENT_ID_SECRET");
                else if (ex is ServiceException)
                    throw;
            }
            return importStatus;
        }
        private PatientBO ImportXml(byte[] fileContent, string pharmacypatientid, string patientEmail, string patientPhone, string filename, string fileContentType, SessionContext sessioncontext, string accessToken)
        {
            try
            {
                _logger.LogInformation($"ImportXml: pharmacypatientid {pharmacypatientid} patientEmail {patientEmail} patientPhone {patientPhone }");
                if (sessioncontext.RoleCode != RoleCodes.Pharmacist && sessioncontext.RoleCode != RoleCodes.Pharmacy)
                    throw new ServiceException(ClientSideErrors.INVALID_USER_APPLICATION_ROLE);
                var pharmacyid = sessioncontext.RoleCode == RoleCodes.Pharmacist ? _pharmacistRepository.GetByID(sessioncontext.AppUserID).PharmacyID : sessioncontext.AppUserID;
                _logger.LogInformation($"ImportXml: pharmacyid {pharmacyid}");
                using (ApiClient apiClient = new ApiClient(_webApiConf.PatientServiceBaseUrl))
                {
                    apiClient.SetBearerToken(accessToken);
                    apiClient.SetCustomHeader("NambayaSession", JsonSerializer.Serialize(sessioncontext));
                    var res = apiClient.InternalServicePostAsync($"api/v1/patient/ImportXml?pharmacypatientid={pharmacypatientid}&patientemail={patientEmail}&patientphone={patientPhone}&fileContentType={fileContentType}&filename={filename}", JsonSerializer.Serialize(fileContent)).Result;
                    return JsonSerializer.Deserialize<PatientBO>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
    }
}
