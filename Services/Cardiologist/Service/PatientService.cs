using AutoMapper;
using Cardiologist.Contracts.Interfaces;
using Cardiologist.Contracts.Models;
using Cardiologist.Repository.Interfaces;
using Common.BusinessObjects;
using Common.BusinessObjects.Interfaces;
using Common.Infrastructure;
using Common.Infrastructure.Exceptions;
using Common.Infrastructure.Extensions;
using Common.Infrastructure.Helpers;
using Common.Services;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Patient.Contracts.Models;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;

namespace Cardiologist.Service
{
    public class PatientService : BaseService, IPatientService
    {
        private readonly IMapper _mapper;
        private readonly WebServiceConfiguration _webApiConf;
        private readonly ICardiologistRepository _cardiologistRepository;
        private readonly ISignatureService _signatureService;
        private readonly ICommonUserServce _commonUserServce;
        private readonly ILogger<PatientService> _logger;
        public PatientService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _webApiConf = serviceProvider.GetRequiredService<WebServiceConfiguration>();
            _mapper = serviceProvider.GetRequiredService<IMapper>();
            _cardiologistRepository = serviceProvider.GetRequiredService<ICardiologistRepository>();
            _signatureService = serviceProvider.GetRequiredService<ISignatureService>();
            _commonUserServce = serviceProvider.GetRequiredService<ICommonUserServce>();
            _logger = serviceProvider.GetRequiredService<ILogger<PatientService>>();
        }
        #region Patient Functions 
        public PagedResults<PatientBO> GetPatients(int limit, int offset, string orderby, string filter)
        {
            _logger.LogInformation($"GetPatients: Filter limit : {limit}, offset {offset}, orderby: {orderby}, filter {filter} ");

            var sessioncontext = GetSessionContext();
            if (sessioncontext.RoleCode != RoleCodes.Cardiologist)
                throw new ServiceException(ClientSideErrors.INVALID_USER_APPLICATION_ROLE);
            string order = orderby == null ? "" : $"&orderby={orderby}";
            string sFilter = filter == null ? "" : $"&filter={filter}";
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/patient/pharmacy/0?limit={limit}&offset={offset}{sFilter}").Result;
                    return JsonSerializer.Deserialize<PagedResults<PatientBO>>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                if (ex.InnerException != null)
                {
                    _logger.LogError(ex, ex.InnerException.Message);
                    throw ex.InnerException;
                }
                    
                throw;
            }
        }
        public PagedResults<PatientBO> GetPatientCases(int limit, int offset, string orderby, string filter)
        {
            _logger.LogInformation($"GetPatientCases: Filter limit : {limit}, offset {offset}, orderby: {orderby}, filter {filter} ");

            var sessioncontext = GetSessionContext();
            if (sessioncontext.RoleCode != RoleCodes.Cardiologist)
                throw new ServiceException(ClientSideErrors.INVALID_USER_APPLICATION_ROLE);
            string order = orderby == null ? "" : $"&orderby={orderby}";
            string sFilter = filter == null ? "" : $"&filter={filter}";
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/patient/cases/pharmacy/0?limit={limit}&offset={offset}{sFilter}").Result;
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
            _logger.LogInformation($"GetPatientbyID: Id is {id} ");

            try
            {
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
            _logger.LogInformation($"GetPatientbyCaseID: Id is {patientcaseid} ");

            try
            {
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
            _logger.LogInformation($"GetPatientbyCaseID: Id is {patientcaseid} ");
            try
            {
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
            _logger.LogInformation($"UpdatePatient: Patient  {Newtonsoft.Json.JsonConvert.SerializeObject(patient)} ");

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
        public PatientCaseBO UpdatePatientCase(PatientCaseBO patientCaseBO)
        {
            try
            {
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
        #endregion
        #region Evaluation Functions
        public byte[] DownloadEDFFile(long patientcaseid)
        {
            _logger.LogInformation($"DownloadEDFFile: patientcaseid {patientcaseid}");

            try
            {
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
        public byte[] DownloadDetailReport(long patientcaseid)
        {
            _logger.LogInformation($"DownloadDetailReport: patientcaseid {patientcaseid}");
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl, TimeSpan.FromMinutes(15)))
                {
                    var res = apiClient.DownloadGetAsync($"api/v1/patient/downloadecgfile?patientcaseid={patientcaseid}").Result;
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
        public DetailEvaluationBO GetDetailReport(long patientcaseid)
        {
            _logger.LogInformation($"GetDetailReport: patientcaseid {patientcaseid}");

            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl, TimeSpan.FromMinutes(15)))
                {
                    var res = apiClient.DownloadGetAsync($"api/v1/patient/getecgreportdata?patientcaseid={patientcaseid}").Result;
                    return JsonSerializer.Deserialize<DetailEvaluationBO>(res,
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
        public DetailEvaluationBO UpdateDetailEvaluationData(DetailEvaluationBO detailEvaluationBO)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl, TimeSpan.FromMinutes(15)))
                {
                    var res = apiClient.InternalServicePutAsync($"api/v1/patient/updateecgreportdata", JsonSerializer.Serialize(detailEvaluationBO)).Result;
                    return JsonSerializer.Deserialize<DetailEvaluationBO>(res,
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
        public void UploadDetailEvaluationFile(DetailEvaluationBO detailEvaluationBO, byte[] fileData)
        {
            try
            {
                var sessioncontext = GetSessionContext();
                if (sessioncontext.RoleCode != RoleCodes.Cardiologist && sessioncontext.RoleCode != RoleCodes.Nurse)
                    throw new ServiceException(ClientSideErrors.USER_NOT_AUTHORIZED);
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl, TimeSpan.FromMinutes(15)))
                {
                    var res = apiClient.InternalServicePostFileAsync($"api/v1/patient/uploadecgfiledata", detailEvaluationBO.FileName, fileData, detailEvaluationBO).Result;
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
        public string GenerateDESignatureOTP(long patientcaseid)
        {
            _logger.LogInformation($"GenerateDESignatureOTP: patientcaseid {patientcaseid}");

            SessionContext sessionContext = GetSessionContext();
            var user = _mapper.Map<CardiologistBO>(_cardiologistRepository.GetByEmail(sessionContext.LoginName));
            var rabbitmq = _serviceProvider.GetRequiredService<RabbitMQClient>();
            int digit = 8;
            var otp = new Random().Next((int)Math.Pow(10, digit - 1) + 1, (int)Math.Pow(10, digit) - 1).ToString("########");
            var sid = DateTime.Now.Ticks.ToString();
            var hash = string.Format("{0}:{1}", sid, otp).Sha256();
            var token = JwtWrapper.GenerateToken(ConfigurationConsts.DESignatureVerificationKey.Sha256(),
                new Claim[]
                {
                  new Claim("sid", sid),
                  new Claim("otp_email", user.Email),
                  new Claim("otp_name", user.Name??""),
                  new Claim("otp_patientcaseid",patientcaseid.ToString()),
                  new Claim("otp_hash", hash),
                }, 300);
            rabbitmq.SendMessage(KnownChannels.DETAIL_REPORT_SIGN_EVENT_CHANNEL, new PatientUserOtp()
            {
                AppUserID = user.ID,
                Email = user.Email,
                Name = user.Name,
                Phone = user.Phone,
                OTP = otp,
                SMS = user.PhoneVerified && _commonUserServce.GetSettings(user.Email).FirstOrDefault(p => p.Code == UserSettingCodes.FACTOR_NOTIFICATION_TYPE)?.Value == NotificationType.SMS.ToString(),
                SessionContext = sessionContext,
                Patient = GetPatientbyCaseID(patientcaseid)
            });
            return token;
        }
        public void VerifyECGFileUploadSignature(VerifyECGUploadOtpRequest req, DetailEvaluationBO detailEvaluationBO)
        {
            try
            {
                SessionContext sessionContext = GetSessionContext();
                var user = _cardiologistRepository.GetByEmail(req.Email);
                _logger.LogInformation($"VerifyECGFileUploadSignature: user {Newtonsoft.Json.JsonConvert.SerializeObject(user)}");

                var principle = JwtWrapper.GetClaimsPrincipal(ConfigurationConsts.DESignatureVerificationKey.Sha256(), req.Token);
                if (principle?.Identity?.IsAuthenticated == true)
                {
                    var claimsIdentity = principle.Identity as ClaimsIdentity;
                    var claims = claimsIdentity.Claims.ToArray();
                    var sid = claims.Single(c => c.Type == "sid").Value;
                    var hash = claims.Single(c => c.Type == "otp_hash").Value;
                    if (hash == string.Format("{0}:{1}", sid, req.OTP.ToString()).Sha256() &&
                        claims.Single(c => c.Type == "otp_email").Value == req.Email &&
                        claims.Single(c => c.Type == "otp_patientcaseid").Value == req.PatientCaseId.ToString())
                    {
                        var sig = _signatureService.GetSignatures();
                        if (sig == null)
                            throw new ServiceException(ClientSideErrors.SIGNATURE_NOT_FOUND);
                        using (var _unitOfWork = _serviceProvider.GetRequiredService<Common.DataAccess.Interfaces.IUnitOfWork>())
                        {
                            try
                            {
                                SignDetailReport(detailEvaluationBO, sig);
                                _unitOfWork.Commit();
                                return;
                            }
                            catch (Exception ex)
                            {
                                _unitOfWork.Rollback();
                                _logger.LogError(ex, ex.Message);
                                throw;
                            }
                        }
                    }
                }
                else
                {
                    _logger.LogWarning("User identity is not authenticated ");
                }
            }
            catch (ServiceException)
            {
                throw;
            }
            catch
            {
            }
            throw new ServiceException(ClientSideErrors.OTP_VERIFICATION_FAILED);
        }
        public void VerifyExistingECGFileWithSignature(VerifyECGUploadOtpRequest req)
        {
            try
            {
                SessionContext sessionContext = GetSessionContext();
                var user = _cardiologistRepository.GetByEmail(req.Email);
                _logger.LogInformation($"VerifyExistingECGFileWithSignature: user email  {req.Email}");

                var principle = JwtWrapper.GetClaimsPrincipal(ConfigurationConsts.DESignatureVerificationKey.Sha256(), req.Token);
                if (principle?.Identity?.IsAuthenticated == true)
                {
                    var claimsIdentity = principle.Identity as ClaimsIdentity;
                    var claims = claimsIdentity.Claims.ToArray();
                    var sid = claims.Single(c => c.Type == "sid").Value;
                    var hash = claims.Single(c => c.Type == "otp_hash").Value;
                    if (hash == string.Format("{0}:{1}", sid, req.OTP.ToString()).Sha256() &&
                        claims.Single(c => c.Type == "otp_email").Value == req.Email &&
                        claims.Single(c => c.Type == "otp_patientcaseid").Value == req.PatientCaseId.ToString())
                    {
                        var sig = _signatureService.GetSignatures();
                        if (sig == null)
                            throw new ServiceException(ClientSideErrors.SIGNATURE_NOT_FOUND);
                        using (var _unitOfWork = _serviceProvider.GetRequiredService<Common.DataAccess.Interfaces.IUnitOfWork>())
                        {
                            try
                            {
                                DetailEvaluationBO detailEvaluationBO = GetDetailReport(req.PatientCaseId);
                                detailEvaluationBO.ResultsPath = $"ECG/{req.PatientCaseId}/{Guid.NewGuid()}";
                                detailEvaluationBO.Notes = req.Notes;
                                detailEvaluationBO.NotesTypeID = req.NotesTypeId;
                                detailEvaluationBO.FileData = DownloadDetailReport(req.PatientCaseId);
                                SignDetailReport(detailEvaluationBO, sig);
                                _unitOfWork.Commit();
                                return;
                            }
                            catch (Exception ex)
                            {
                                _unitOfWork.Rollback();
                                _logger.LogError(ex, ex.Message);
                                throw;
                            }
                        }
                    }
                }
                else
                {
                    _logger.LogWarning("User identity is not authenticated ");
                }
            }
            catch (ServiceException)
            {
                throw;
            }
            catch
            {
            }
            throw new ServiceException(ClientSideErrors.OTP_VERIFICATION_FAILED);
        }
        private string GetCardioQRCode(string qrText)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            using (MemoryStream stream = new MemoryStream())
            {
                Bitmap qrCodeImage = qrCode.GetGraphic(20);
                qrCodeImage.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return Convert.ToBase64String(stream.ToArray());
            }
        }
        private void SignDetailReport(DetailEvaluationBO detailEvaluationBO, SignaturesBO sig)
        {
            SessionContext sessionContext = GetSessionContext();
            var user = _cardiologistRepository.GetByEmail(sessionContext.LoginName);
            //var detailReport = _patientService.DownloadDetailReport(patientcaseid);
            var docAuthor = "Nambaya System";

            var pdfFiles = new List<byte[]>();
            pdfFiles.Add(detailEvaluationBO.FileData);
            using (var stream = new MemoryStream())
            {
                _logger.LogInformation($"SignDetailReport:create a StyleSheet");
                // create a StyleSheet
                var styles = new StyleSheet();
                styles.LoadTagStyle("ul", "indent", "10");
                // step 1
                var document = new Document();
                // step 2
                PdfWriter.GetInstance(document, stream);
                // step 3
                document.AddAuthor(docAuthor);
                document.SetMargins(50, 50, 250, 5);
                document.Open();
                // step 4
                var htmlString = File.ReadAllText(@"signaturepage.html").Replace("##IMAGEDATA##", sig.FileDataString)
                    .Replace("##CARDIOLOGISTNAME##", user.FirstName + " " + user.LastName).Replace("##CREATIONDATE##", DateTime.Now.ToString("dd.MM.yyyy"))
                    .Replace("##CARDIOLOGISTDOCTORID##", user.DoctorID ?? "")
                    .Replace("##QRIMAGEDATA##", GetCardioQRCode($"Cardiologist ID: {user.DoctorID} " +
                    $"\nCardiologist Name: {user.FirstName + " " + user.LastName}" +
                    $"\nCaseID: { detailEvaluationBO.PatientCaseID.ToString("D6") }"));
                var objects = HtmlWorker.ParseToList(
                    new StringReader(htmlString),
                    styles
                );
                foreach (IElement element in objects)
                {
                    document.Add(element);
                }

                document.Close();
                pdfFiles.Add(stream.ToArray());
                _logger.LogInformation($"SignDetailReport:Created");

            }

            using (var finalStream = new MemoryStream())
            {
                // step 1
                var document1 = new Document();
                // step 2
                var pdfCopy = new PdfCopy(document1, finalStream);
                // step 3
                document1.AddAuthor(docAuthor);
                document1.Open();
                // step 4
                foreach (var pdf in pdfFiles)
                {
                    var reader = new PdfReader(pdf);
                    var n = reader.NumberOfPages;
                    for (var page = 0; page < n;)
                    {
                        pdfCopy.AddPage(pdfCopy.GetImportedPage(reader, ++page));
                    }
                    reader.Close();
                }

                document1.Close();
                pdfCopy.Close();

                UploadDetailEvaluationFile(
                    new DetailEvaluationBO()
                    {
                        FileName = detailEvaluationBO.FileName,
                        PatientCaseID = detailEvaluationBO.PatientCaseID,
                        ContentType = detailEvaluationBO.ContentType,
                        ResultsPath = detailEvaluationBO.ResultsPath,
                        FileLength = finalStream.Length,
                        Notes = detailEvaluationBO.Notes,
                        NotesTypeID = detailEvaluationBO.NotesTypeID,
                        IsSigned = true
                    }, finalStream.ToArray());
                _logger.LogInformation($"UploadDetailEvaluationFile completed");

            }
        }
        #endregion
        #region Additional Info
        public PatientAdditionalInfoBO GetPatientAdditionalInfoByCaseID(long patientcaseid)
        {
            try
            {
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

        #endregion
        #region dashboard
        public GoalCompletedBO GetGoalcompletedPercent()
        {
            try
            {
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
        public List<MonthlyCasesBO> GetMonthlyCasesCompleted()
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/patient/dashboard/monthlycasescompleted").Result;
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
        public long GetTotalCardiologistAssignedCases()
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/patient/dashboard/totalcardiologistassignedcases").Result;
                    var d = JsonSerializer.Deserialize<long>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    return d;
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        public long GetTotalCardiologistActiveCases()
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/patient/dashboard/totalcardiologistactivecases").Result;
                    var d = JsonSerializer.Deserialize<long>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    return d;
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        public List<CaseStatsBO> GetCardiologistNotesStats()
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PatientServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/patient/dashboard/cardiologistnotesstats").Result;
                    return JsonSerializer.Deserialize<List<CaseStatsBO>>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
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
