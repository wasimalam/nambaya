using AutoMapper;
using Common.BusinessObjects;
using Common.BusinessObjects.Interfaces;
using Common.Infrastructure;
using Common.Infrastructure.Exceptions;
using Common.Infrastructure.Extensions;
using Common.Infrastructure.Helpers;
using Common.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Patient.Contracts.Models;
using Pharmacist.Contracts.Interfaces;
using Pharmacist.Contracts.Models;
using Pharmacist.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;

namespace Pharmacist.Service
{
    public class PatientService : BaseService, IPatientService
    {
        private readonly IMapper _mapper;
        private readonly WebServiceConfiguration _webApiConf;
        private readonly IPharmacistRepository _pharmacistRepository;
        private readonly IPharmacyRepository _pharmacyRepository;
        private readonly ICommonUserServce _commonUserServce;
        private readonly ILogger<PatientService> _logger;
        public PatientService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _webApiConf = serviceProvider.GetRequiredService<WebServiceConfiguration>();
            _mapper = serviceProvider.GetRequiredService<IMapper>();
            _pharmacistRepository = serviceProvider.GetRequiredService<IPharmacistRepository>();
            _pharmacyRepository = serviceProvider.GetRequiredService<IPharmacyRepository>();
            _commonUserServce = serviceProvider.GetRequiredService<ICommonUserServce>();
            _logger = serviceProvider.GetRequiredService<ILogger<PatientService>>();
        }
        public PagedResults<PatientBO> GetPatientCases(int limit, int offset, string orderby, string filter)
        {
            _logger.LogInformation($"GetPatientCases: limit {limit} offset {offset} orderby {orderby} filter {filter}");
            var sessioncontext = GetSessionContext();
            if (sessioncontext.RoleCode != RoleCodes.Pharmacist && sessioncontext.RoleCode != RoleCodes.Pharmacy)
                throw new ServiceException(ClientSideErrors.INVALID_USER_APPLICATION_ROLE);
            string order = orderby == null ? "" : $"&orderby={orderby}";
            string sFilter = filter == null ? "" : $"&filter={filter}";
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/patient?limit={limit}&offset={offset}{sFilter}").Result;
                    return JsonSerializer.Deserialize<PagedResults<PatientBO>>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        public PatientBO GetPatientbyID(long id)
        {
            try
            {
                _logger.LogInformation($"GetPatientbyID: patient id {id}");
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/patient/{id}").Result;
                    return JsonSerializer.Deserialize<PatientBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message == ClientSideErrors.RESOURCE_NOT_FOUND)
                    return null;
                throw ex.InnerException;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        public PatientBO GetPatientbyCaseID(long patientcaseid)
        {
            try
            {
                _logger.LogInformation($"GetPatientbyCaseID: patient case id {patientcaseid}");
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/patient/cases/{patientcaseid}").Result;
                    return JsonSerializer.Deserialize<PatientBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message == ClientSideErrors.RESOURCE_NOT_FOUND)
                    return null;
                throw ex.InnerException;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        public PatientCaseBO GetPatientCasebyID(long patientcaseid)
        {
            try
            {
                _logger.LogInformation($"GetPatientCasebyID: patient case id {patientcaseid}");
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/patient/getpatientcase/{patientcaseid}").Result;
                    return JsonSerializer.Deserialize<PatientCaseBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message == ClientSideErrors.RESOURCE_NOT_FOUND)
                    return null;
                throw ex.InnerException;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        public PatientBO AddPatient(PatientBO patient)
        {
            try
            {
                _logger.LogInformation($"AddPatient: patient {Newtonsoft.Json.JsonConvert.SerializeObject(patient)}");
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServicePostAsync($"api/v1/patient", JsonSerializer.Serialize(patient)).Result;
                    return JsonSerializer.Deserialize<PatientBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        public PatientBO UpdatePatient(PatientBO patient)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServicePutAsync($"api/v1/patient", JsonSerializer.Serialize(patient)).Result;
                    return JsonSerializer.Deserialize<PatientBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }

        public string GenerateDeActivatePatientVerification(long patientid)
        {
            _logger.LogInformation($"GenerateDeActivatePatientVerification: patient id {patientid}");
            SessionContext sessionContext = GetSessionContext();
            if (sessionContext.RoleCode != RoleCodes.Pharmacist && sessionContext.RoleCode != RoleCodes.Pharmacy)
                throw new ServiceException(ClientSideErrors.INVALID_USER_ACTION);
            var pharmacy = _mapper.Map<PharmacyBO>(_pharmacyRepository.GetByEmail(sessionContext.LoginName));
            var patient = GetPatientbyID(patientid);
            if(patient == null || patient.PharmacyID != sessionContext.PharmacyID || !patient.IsActive)
                throw new ServiceException(ClientSideErrors.INVALID_USER_ACTION);

            PatientUserOtp userOtp = new PatientUserOtp()
            {
                SessionContext = sessionContext,
                Patient = patient
            };
            if (pharmacy != null)
            {
                userOtp.AppUserID = pharmacy.ID;
                userOtp.Name = pharmacy.Name;
                userOtp.Email = pharmacy.Email;
                userOtp.Phone = pharmacy.Phone;
                userOtp.SMS = pharmacy.PhoneVerified && _commonUserServce.GetSettings(pharmacy.Email).FirstOrDefault(p => p.Code == UserSettingCodes.FACTOR_NOTIFICATION_TYPE)?.Value == NotificationType.SMS.ToString();
            }
            else
            {
                var pharmacist = _mapper.Map<PharmacistBO>(_pharmacistRepository.GetByEmail(sessionContext.LoginName));
                if (pharmacist != null)
                {
                    userOtp.AppUserID = pharmacist.ID;
                    userOtp.Name = pharmacist.Name;
                    userOtp.Email = pharmacist.Email;
                    userOtp.Phone = pharmacist.Phone;
                    userOtp.SMS = pharmacist.PhoneVerified && _commonUserServce.GetSettings(pharmacist.Email).FirstOrDefault(p => p.Code == UserSettingCodes.FACTOR_NOTIFICATION_TYPE)?.Value == NotificationType.SMS.ToString();
                }
                else
                    throw new ServiceException(ClientSideErrors.INVALID_USER_ACTION);
            }
            var rabbitmq = _serviceProvider.GetRequiredService<RabbitMQClient>();
            int digit = 8;
            userOtp.OTP = new Random().Next((int)Math.Pow(10, digit - 1) + 1, (int)Math.Pow(10, digit) - 1).ToString("########");
            var sid = DateTime.Now.Ticks.ToString();
            var hash = string.Format("{0}:{1}", sid, userOtp.OTP).Sha256();
            _logger.LogInformation("GenerateDeActivatePatientVerification: generating token ");
            var token = JwtWrapper.GenerateToken(ConfigurationConsts.DeActivatePatientVerificationKey.Sha256(), new Claim[]
                {
                  new Claim("sid", sid),
                  new Claim("otp_email", sessionContext.LoginName),
                  new Claim("otp_patientid", patientid.ToString()),
                  new Claim("otp_hash", hash),
                }, 120);
            rabbitmq.SendMessage(KnownChannels.PATIENT_DEACTIVATE_VERIFICATION_CHANNEL, userOtp);
            return token;
        }
        public void VerifyDeActivatePatient(VerifyDeactivatePatientOtpRequest req)
        {
            try
            {
                SessionContext sessionContext = GetSessionContext();
                if (sessionContext.LoginName.ToLower() != req.Email.ToLower())
                    throw new ServiceException(ClientSideErrors.OTP_VERIFICATION_FAILED);
                var principle = JwtWrapper.GetClaimsPrincipal(ConfigurationConsts.DeActivatePatientVerificationKey.Sha256(), req.Token);
                if (principle?.Identity?.IsAuthenticated == true)
                {
                    var claimsIdentity = principle.Identity as ClaimsIdentity;
                    var claims = claimsIdentity.Claims.ToArray();
                    var sid = claims.Single(c => c.Type == "sid").Value;
                    var hash = claims.Single(c => c.Type == "otp_hash").Value;
                    if (hash == string.Format("{0}:{1}", sid, req.OTP.ToString()).Sha256() &&
                        claims.Single(c => c.Type == "otp_email").Value == req.Email &&
                        claims.Single(c => c.Type == "otp_patientid").Value == req.PatientId.ToString())
                    {
                        _logger.LogInformation("OTP request verify... Deactivating patient");
                        DeActivatePatient(req.PatientId);
                        return;
                    }
                }
                else
                {
                    _logger.LogWarning("User identity is not authentcated");
                }
            }
            catch
            {
            }
            throw new ServiceException(ClientSideErrors.OTP_VERIFICATION_FAILED);
        }
        private void DeActivatePatient(long patientId)
        {
            try
            {
                _logger.LogInformation($"DeActivatePatient: patient case id {patientId}");
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var deviceService = _serviceProvider.GetRequiredService<IDeviceService>();
                    var res = apiClient.InternalServiceGetAsync($"api/v1/patient/getpatientcases/{patientId}").Result;
                    var cases = JsonSerializer.Deserialize<List<PatientCaseBO>>(res,
                     new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    foreach (var c in cases)
                    {
                        if (c.StatusID == CaseStatus.DeviceAllocated)
                        {
                            Pharmacist.Contracts.Models.DeviceAssignmentBO da = deviceService.GetDeviceAssignmentByPatientCaseID(c.ID);
                            if (da.IsAssigned)
                            {
                                da.DeviceStatusID = DeviceStatus.Available;
                                da.IsAssigned = false;
                                deviceService.AssignDevice(da);
                            }
                        }
                    }
                    res = apiClient.InternalServicePutAsync($"api/v1/patient/deactivatepatient/{patientId}", new { patientId = patientId }).Result;
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }

        public PatientCaseBO UpdatePatientCase(PatientCaseBO patientCaseBO)
        {
            try
            {
                _logger.LogInformation($"UpdatePatientCase: patient cases {Newtonsoft.Json.JsonConvert.SerializeObject(patientCaseBO)}");
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServicePutAsync($"api/v1/patient/cases", patientCaseBO).Result;
                    return JsonSerializer.Deserialize<PatientCaseBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        public PatientBO ImportXml(byte[] fileContent, string pharmacypatientid, string filename, string fileContentType)
        {
            try
            {
                _logger.LogInformation($"ImportXml: pharmacypatientid {pharmacypatientid} filename {filename} ");
                var sessioncontext = GetSessionContext();
                if (sessioncontext == null)
                {
                    sessioncontext = (Common.BusinessObjects.SessionContext)Thread.GetData(Thread.GetNamedDataSlot("NambayaSession"));
                }
                if (sessioncontext.RoleCode != RoleCodes.Pharmacist && sessioncontext.RoleCode != RoleCodes.Pharmacy)
                    throw new ServiceException(ClientSideErrors.INVALID_USER_APPLICATION_ROLE);
                var pharmacyid = sessioncontext.RoleCode == RoleCodes.Pharmacist ? _pharmacistRepository.GetByID(sessioncontext.AppUserID).PharmacyID : sessioncontext.AppUserID;
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServicePostAsync($"api/v1/patient/ImportXml?pharmacypatientid={pharmacypatientid}&fileContentType={fileContentType}&filename={filename}", JsonSerializer.Serialize(fileContent)).Result;
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
        #region Evaluation
        public PatientEDFFileBO GetPatientEDFFile(long patientcaseid)
        {
            try
            {
                _logger.LogInformation($"GetPatientEDFFile: Patient case id {patientcaseid}");
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl, TimeSpan.FromMinutes(15)))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/patient/getpatientedffile?patientcaseid={patientcaseid}").Result;
                    return JsonSerializer.Deserialize<PatientEDFFileBO>(res,
                       new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message == ClientSideErrors.RESOURCE_NOT_FOUND)
                    return null;
                throw ex.InnerException;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        public byte[] DownloadEDFFile(long patientcaseid)
        {
            try
            {
                _logger.LogInformation($"Download edf file of patient case id {patientcaseid}");
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl, TimeSpan.FromMinutes(15)))
                {
                    var res = apiClient.DownloadGetAsync($"api/v1/patient/downloadedffile?patientcaseid={patientcaseid}").Result;
                    return res;
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        public void UploadEDFFile(PatientEDFFileBO patientEDFFileBO, byte[] fileData)
        {
            try
            {
                var sessioncontext = GetSessionContext();
                if (sessioncontext.RoleCode != RoleCodes.Pharmacist && sessioncontext.RoleCode != RoleCodes.Pharmacy)
                    throw new ServiceException(ClientSideErrors.USER_NOT_AUTHORIZED);
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl, TimeSpan.FromMinutes(15)))
                {
                    var res = apiClient.InternalServicePostFileAsync($"api/v1/patient/uploadedffiledata", patientEDFFileBO.FileName, fileData, patientEDFFileBO).Result;
                }
                return;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        public QuickEvaluationFileBO DownloadQuickEvalFile(long patientcaseid)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl, TimeSpan.FromMinutes(15)))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/patient/downloadquickevaluationfiledata?patientcaseid={patientcaseid}").Result;
                    return JsonSerializer.Deserialize<QuickEvaluationFileBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        public List<string> DownloadQuickEvalImages(long patientcaseid)
        {
            List<string> imageBase64String = new List<string>();
            try
            {
                _logger.LogInformation($"DownloadQuickEvalImages: patient case id {patientcaseid}");
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl, TimeSpan.FromMinutes(15)))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/patient/downloadquickevaluationfiledata?patientcaseid={patientcaseid}").Result;
                    var zipfile = JsonSerializer.Deserialize<QuickEvaluationFileBO>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    MemoryStream ms = new MemoryStream(zipfile.FileData);
                    var unzippedStreams = CompressionHelper.UnZipStream(ms);
                    foreach (var unzipStream in unzippedStreams)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            unzipStream.CopyTo(memoryStream);
                            imageBase64String.Add(System.Convert.ToBase64String(memoryStream.ToArray()));
                        }
                    }
                }
                return imageBase64String;
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message == ClientSideErrors.RESOURCE_NOT_FOUND)
                    return null;
                throw ex.InnerException;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        public void UploadQuickEvalFile(QuickEvaluationFileBO quickEvaluationFileBO, byte[] fileData)
        {
            try
            {
                var sessioncontext = GetSessionContext();
                if (sessioncontext.RoleCode != RoleCodes.Pharmacist && sessioncontext.RoleCode != RoleCodes.Pharmacy)
                    throw new ServiceException(ClientSideErrors.INVALID_USER_APPLICATION_ROLE);
                var pharmacyid = sessioncontext.RoleCode == RoleCodes.Pharmacist ? _pharmacistRepository.GetByID(sessioncontext.AppUserID).PharmacyID : sessioncontext.AppUserID;
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl, TimeSpan.FromMinutes(15)))
                {
                    var res = apiClient.InternalServicePostFileAsync($"api/v1/patient/uploadquickevaluationfiledata", quickEvaluationFileBO.FileName, fileData, quickEvaluationFileBO).Result;
                }
                return;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        public QuickEvaluationResultBO GetQuickEvaluationResultByCaseID(long patientcaseid)
        {
            try
            {
                _logger.LogInformation($"GetQuickEvaluationResultByCaseID: patient case id {patientcaseid}");
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/patient/cases/quickevaluationresult/{patientcaseid}").Result;
                    return JsonSerializer.Deserialize<QuickEvaluationResultBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message == ClientSideErrors.RESOURCE_NOT_FOUND)
                    return null;
                throw ex.InnerException;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        public QuickEvaluationResultBO AddQuickEvaluationResult(QuickEvaluationResultBO quickEvaluationResult)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServicePostAsync($"api/v1/patient/cases/quickevaluationresult", JsonSerializer.Serialize(quickEvaluationResult)).Result;
                    return JsonSerializer.Deserialize<QuickEvaluationResultBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        public QuickEvaluationResultBO UpdateQuickEvaluationResult(QuickEvaluationResultBO quickEvaluationResult)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServicePutAsync($"api/v1/patient/cases/quickevaluationresult", JsonSerializer.Serialize(quickEvaluationResult)).Result;
                    return JsonSerializer.Deserialize<QuickEvaluationResultBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        #endregion
        #region Additional Info
        public PatientAdditionalInfoBO GetPatientAdditionalInfoByCaseID(long patientcaseid)
        {
            try
            {
                _logger.LogInformation($"GetPatientAdditionalInfoByCaseID: patient case id {patientcaseid}");
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/patient/cases/additionalinfo/{patientcaseid}").Result;
                    return JsonSerializer.Deserialize<PatientAdditionalInfoBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message == ClientSideErrors.RESOURCE_NOT_FOUND)
                    return null;
                throw ex.InnerException;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }

        public PatientAdditionalInfoBO AddPatientAdditionalInfo(PatientAdditionalInfoBO patientAdditionalInfo)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServicePostAsync($"api/v1/patient/cases/additionalinfo", JsonSerializer.Serialize(patientAdditionalInfo)).Result;
                    return JsonSerializer.Deserialize<PatientAdditionalInfoBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }

        public PatientAdditionalInfoBO UpdatePatientAdditionalInfo(PatientAdditionalInfoBO patientAdditionalInfo)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServicePutAsync($"api/v1/patient/cases/additionalinfo", JsonSerializer.Serialize(patientAdditionalInfo)).Result;
                    return JsonSerializer.Deserialize<PatientAdditionalInfoBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        #endregion
        #region drug related functions
        public List<DrugGroupBO> GetDrugGroups(long patientcaseid)
        {
            try
            {
                _logger.LogInformation($"GetDrugGroups: patient case id {patientcaseid}");
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/patient/druggroups/patientcase/{patientcaseid}").Result;
                    return JsonSerializer.Deserialize<List<DrugGroupBO>>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message == ClientSideErrors.RESOURCE_NOT_FOUND)
                    return null;
                throw ex.InnerException;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        public DrugGroupBO GetDrugGroup(long druggroupid)
        {
            try
            {
                _logger.LogInformation($"GetDrugGroup: drug group id {druggroupid}");
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/patient/druggroups/{druggroupid}").Result;
                    return JsonSerializer.Deserialize<DrugGroupBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message == ClientSideErrors.RESOURCE_NOT_FOUND)
                    return null;
                throw ex.InnerException;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }

        public DrugGroupBO AddDrugGroup(DrugGroupBO drugGroupBO)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServicePostAsync($"api/v1/patient/druggroups", JsonSerializer.Serialize(drugGroupBO)).Result;
                    return JsonSerializer.Deserialize<DrugGroupBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        public DrugGroupBO UpdateDrugGroup(DrugGroupBO drugGroupBO)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServicePutAsync($"api/v1/patient/druggroups", JsonSerializer.Serialize(drugGroupBO)).Result;
                    return JsonSerializer.Deserialize<DrugGroupBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message == ClientSideErrors.RESOURCE_NOT_FOUND)
                    return null;
                throw ex.InnerException;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }

        public void DeleteDrugGroup(DrugGroupBO drugGroupBO)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.DeleteInternalAsync($"api/v1/patient/druggroups", JsonSerializer.Serialize(drugGroupBO)).Result;
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }

        public DrugDetailsBO AddDrugDetails(DrugDetailsBO drugDetailsBO)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServicePostAsync($"api/v1/patient/druggroups/details", JsonSerializer.Serialize(drugDetailsBO)).Result;
                    return JsonSerializer.Deserialize<DrugDetailsBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }

        public DrugDetailsBO UpdateDrugDetails(DrugDetailsBO drugDetailsBO)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServicePutAsync($"api/v1/patient/druggroups/details", JsonSerializer.Serialize(drugDetailsBO)).Result;
                    return JsonSerializer.Deserialize<DrugDetailsBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message == ClientSideErrors.RESOURCE_NOT_FOUND)
                    return null;
                throw ex.InnerException;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }

        public void DeleteDrugDetails(DrugDetailsBO drugDetailsBO)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.DeleteInternalAsync($"api/v1/patient/druggroups/details", JsonSerializer.Serialize(drugDetailsBO)).Result;
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }

        public DrugIngredientsBO AddDrugIngredients(DrugIngredientsBO drugIngredientsBO)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServicePostAsync($"api/v1/patient/druggroups/ingredients", JsonSerializer.Serialize(drugIngredientsBO)).Result;
                    return JsonSerializer.Deserialize<DrugIngredientsBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }

        public DrugFreeTextBO AddDrugFreeText(DrugFreeTextBO drugFreeTextBO)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServicePostAsync($"api/v1/patient/druggroups/freetext", JsonSerializer.Serialize(drugFreeTextBO)).Result;
                    return JsonSerializer.Deserialize<DrugFreeTextBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }

        public DrugFreeTextBO UpdateDrugFreeText(DrugFreeTextBO drugFreeTextBO)
        {
            try
            {
                _logger.LogInformation($"UpdateDrugFreeText: {Newtonsoft.Json.JsonConvert.SerializeObject(drugFreeTextBO)}");
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServicePutAsync($"api/v1/patient/druggroups/freetext", JsonSerializer.Serialize(drugFreeTextBO)).Result;
                    return JsonSerializer.Deserialize<DrugFreeTextBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message == ClientSideErrors.RESOURCE_NOT_FOUND)
                    return null;
                throw ex.InnerException;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }

        public void DeleteDrugFreeText(DrugFreeTextBO drugFreeTextBO)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.DeleteInternalAsync($"api/v1/patient/druggroups/freetext", JsonSerializer.Serialize(drugFreeTextBO)).Result;
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }

        public DrugReceipeBO AddDrugReceipe(DrugReceipeBO drugReceipeBO)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServicePostAsync($"api/v1/patient/druggroups/recipe", JsonSerializer.Serialize(drugReceipeBO)).Result;
                    return JsonSerializer.Deserialize<DrugReceipeBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }

        public DrugReceipeBO UpdateDrugReceipe(DrugReceipeBO drugReceipeBO)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServicePutAsync($"api/v1/patient/druggroups/recipe", JsonSerializer.Serialize(drugReceipeBO)).Result;
                    return JsonSerializer.Deserialize<DrugReceipeBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message == ClientSideErrors.RESOURCE_NOT_FOUND)
                    return null;
                throw ex.InnerException;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }

        }

        public void DeleteDrugReceipe(DrugReceipeBO drugReceipeBO)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.DeleteInternalAsync($"api/v1/patient/druggroups/recipe", JsonSerializer.Serialize(drugReceipeBO)).Result;
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }

        #endregion
        #region dashboard
        public object GetPharmacyStats()
        {
            try
            {
                _logger.LogInformation("Getting pharmacy stats");
                var sessioncontext = GetSessionContext();
                if (sessioncontext.RoleCode != RoleCodes.Pharmacist && sessioncontext.RoleCode != RoleCodes.Pharmacy)
                    throw new ServiceException(ClientSideErrors.INVALID_USER_APPLICATION_ROLE);
                var pharmacyid = sessioncontext.RoleCode == RoleCodes.Pharmacist ? _pharmacistRepository.GetByID(sessioncontext.AppUserID).PharmacyID : sessioncontext.AppUserID;
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/patient/dashboard/started/{pharmacyid}").Result;
                    var started = JsonSerializer.Deserialize<long>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    res = apiClient.InternalServiceGetAsync($"api/v1/patient/dashboard/pending/{pharmacyid}").Result;
                    var pending = JsonSerializer.Deserialize<long>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    res = apiClient.InternalServiceGetAsync($"api/v1/patient/dashboard/qecompleted/{pharmacyid}").Result;
                    var qecompleted = JsonSerializer.Deserialize<long>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    res = apiClient.InternalServiceGetAsync($"api/v1/patient/dashboard/detailcompleted/{pharmacyid}").Result;
                    var detailcompleted = JsonSerializer.Deserialize<long>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    return new { started = started, pending = pending, qecompleted = qecompleted, detailcompleted = detailcompleted };
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        public GoalCompletedBO GetGoalcompletedPercent()
        {
            try
            {
                _logger.LogInformation("GetGoalcompletedPercent started");
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/patient/dashboard/goalcompletedpercent").Result;
                    return JsonSerializer.Deserialize<GoalCompletedBO>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        public List<MonthlyCasesBO> GetMonthlyCasesStarted()
        {
            _logger.LogInformation("Entered in GetMonthlyCasesStarted");

            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/patient/dashboard/monthlycasesstarted/").Result;
                    return JsonSerializer.Deserialize<List<MonthlyCasesBO>>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        public List<MonthlyCasesBO> GetMonthlyCasesCompleted()
        {
            try
            {
                _logger.LogInformation("Entered in GetMonthlyCasesCompleted");

                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/patient/dashboard/monthlycasescompleted/").Result;
                    return JsonSerializer.Deserialize<List<MonthlyCasesBO>>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        public List<QEResultStatBO> GetQEResultStats()
        {
            try
            {
                _logger.LogInformation("Entered in GetQEResultStats");
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/patient/dashboard/qeresultstats").Result;
                    return JsonSerializer.Deserialize<List<QEResultStatBO>>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        #endregion
    }
}
