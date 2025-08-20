using AutoMapper;
using Common.DataAccess.Interfaces;
using Common.Infrastructure;
using Common.Infrastructure.Exceptions;
using Common.Infrastructure.Helpers;
using Common.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Patient.Contracts.Interfaces;
using Patient.Contracts.Models;
using Microsoft.Extensions.Logging;
using Patient.Repository.Interfaces;
using Patient.Repository.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
namespace Patient.Service
{
    public class EvaluationService : BaseService, IEvaluationService
    {
        private readonly IMapper _mapper;
        private readonly IPatientService _patientService;
        private readonly RabbitMQClient _rabbitMQClient;
        private readonly WebServiceConfiguration _webApiConf;
        private readonly IPatientEDFFileRepository _patientEDFFileRepository;
        private readonly IDetailEvaluationRepository _detailEvaluationRepository;
        private readonly IQuickEvaluationFileRepository _quickEvaluationFileRepository;
        private readonly IQuickEvaluationResultRepository _quickEvaluationResultRepository;
        private readonly IPatientCasesRepository _patientCasesRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IDeviceAssignmentRepository _deviceAssignmentRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<EvaluationService> _logger;
        public EvaluationService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _mapper = _serviceProvider.GetRequiredService<IMapper>();
            _patientService = serviceProvider.GetRequiredService<IPatientService>();
            _rabbitMQClient = _serviceProvider.GetRequiredService<RabbitMQClient>();
            _webApiConf = _serviceProvider.GetRequiredService<WebServiceConfiguration>();
            _patientEDFFileRepository = _serviceProvider.GetRequiredService<IPatientEDFFileRepository>();
            _patientRepository = _serviceProvider.GetRequiredService<IPatientRepository>();
            _patientCasesRepository = _serviceProvider.GetRequiredService<IPatientCasesRepository>();
            _detailEvaluationRepository = _serviceProvider.GetRequiredService<IDetailEvaluationRepository>();
            _quickEvaluationFileRepository = _serviceProvider.GetRequiredService<IQuickEvaluationFileRepository>();
            _quickEvaluationResultRepository = _serviceProvider.GetRequiredService<IQuickEvaluationResultRepository>();
            _deviceAssignmentRepository = _serviceProvider.GetRequiredService<IDeviceAssignmentRepository>();
            _configuration = _serviceProvider.GetRequiredService<IConfiguration>();
            _logger = serviceProvider.GetRequiredService<ILogger<EvaluationService>>();
        }
        #region EDF and Evaluation file

        public PatientEDFFileBO GetPatientEDFFile(long patientcaseid)
        {
            _logger.LogInformation($"Getting patientEdf file against patient case id  {patientcaseid}");
            var edfFile = _patientEDFFileRepository.GetByPatientCaseId(patientcaseid).FirstOrDefault();
            return _mapper.Map<PatientEDFFileBO>(edfFile);
        }
        public PatientEDFFileBO DownloadEDFFile(long patientcaseid)
        {
            _logger.LogInformation($"DownloadEDFFile: patient case id  {patientcaseid}");
            var edfFile = _patientEDFFileRepository.GetByPatientCaseId(patientcaseid).FirstOrDefault();
            PatientEDFFileBO ret = null;
            if (edfFile != null)
            {
                var sessionContext = GetSessionContext();
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.FileSharingServiceBaseUrl, TimeSpan.FromMinutes(15)))
                {
                    var data = apiClient.DownloadGetAsync($"api/v1/filesharing/{edfFile.FilePath}", "application/x-www-form-urlencoded", "*/*").Result;
                    ret = _mapper.Map<PatientEDFFileBO>(edfFile);
                    ret.FileData = data.Take((int)ret.FileLength).ToArray();
                }
                var patientcase = _patientCasesRepository.GetByID(patientcaseid);

                // moved Unit of Work down because download operation takes a lot of time, thus resulting in transaction timeout exception.
                using (var _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>())
                {
                    try
                    {
                        if (sessionContext != null && (sessionContext.RoleCode == RoleCodes.Cardiologist || sessionContext.RoleCode == RoleCodes.Nurse)
                            && patientcase.StatusID == CaseStatus.QuickEvalCompleted)
                        {
                            patientcase.StatusID = CaseStatus.DetailEvalLocked;
                            patientcase.CardiologistID = sessionContext.CardiologistID;
                            patientcase.UpdatedBy = sessionContext.LoginName;
                            _logger.LogInformation($"Updating patient case by {sessionContext.RoleCode}");
                            _patientCasesRepository.Update(patientcase);
                        }
                        _unitOfWork.Commit();
                    }
                    catch (Exception ex)
                    {
                        _unitOfWork.Rollback();
                        throw ex.InnerException ?? ex;
                    }
                }
            }
            return ret;
        }
        //public byte[] DownloadEDFFileData(long patientcaseid)
        //{
        //    var edfFile = _patientEDFFileRepository.GetByPatientCaseId(patientcaseid).SingleOrDefault();
        //    byte[] ret = null;
        //    if (edfFile != null)
        //    {
        //        var sessionContext = GetSessionContext();
        //        var accessToken = ApiClient.GetAccessToken(_webApiConf.IdentityServerBaseUrl, _apiSecretConf);
        //        ApiClient apiClient = new ApiClient(_webApiConf.FileSharingServiceBaseUrl, TimeSpan.FromMinutes(15));
        //        apiClient.SetBearerToken(accessToken);
        //        apiClient.AddSessionHeader(sessionContext);
        //        var data = apiClient.DownloadGetAsync($"api/v1/filesharing/{edfFile.FilePath}", "application/x-www-form-urlencoded", "*/*").Result;
        //        ret = data.Take(data.Length).ToArray();
        //    }
        //    return ret;
        //}

        public void DeleteEDFFiles(long patientcaseid)
        {
            try
            {
                _logger.LogInformation($"DeleteEDFFiles: patient case id {patientcaseid}");
                var edfFiles = _patientEDFFileRepository.GetByPatientCaseId(patientcaseid);
                foreach (var f in edfFiles)
                {
                    using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.FileSharingServiceBaseUrl, TimeSpan.FromMinutes(15)))
                    {

                        try
                        {
                            var res = apiClient.DeleteInternalAsync($"api/v1/filesharing/{f.FilePath}").Result;
                        }
                        catch (Exception se)
                        {
                            if (se.InnerException != null && se.InnerException is ServiceException && se.InnerException.Message == ClientSideErrors.RESOURCE_NOT_FOUND)
                            {
                            }
                            else throw;
                        }
                        _patientEDFFileRepository.Delete(f);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }
        }

        public void UploadEDFFile(PatientEDFFileBO patientEDFFileBO)
        {
            try
            {
                _logger.LogInformation("Uploading edf file");
                var sessionContext = GetSessionContext();
                patientEDFFileBO.CreatedBy = sessionContext.LoginName;
                DeleteEDFFiles(patientEDFFileBO.PatientCaseID);
                var strUpdateEDFDateTimel = _configuration.GetSection(ConfigurationConsts.UpdateEDFDateTime).Value;
                bool bUpdateEDFDateTime = false;
                bool.TryParse(strUpdateEDFDateTimel, out bUpdateEDFDateTime);
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.FileSharingServiceBaseUrl, TimeSpan.FromMinutes(15)))
                {
                    if (bUpdateEDFDateTime && patientEDFFileBO.FileData.Length > 184)
                    {
                        _logger.LogInformation($"Edf file size {patientEDFFileBO.FileData.Length}");
                        var assign = _deviceAssignmentRepository.GetByCaseId(patientEDFFileBO.PatientCaseID);
                        if (assign != null)
                            Array.ConstrainedCopy(ASCIIEncoding.ASCII.GetBytes(assign.AssignmentDate.ToString("dd.MM.yyHH.mm.ss")), 0,
                                patientEDFFileBO.FileData, 168, 16);
                    }
                    var res = apiClient.PostFileAsync($"api/v1/filesharing/{patientEDFFileBO.FilePath}", Path.GetFileName(patientEDFFileBO.FilePath), patientEDFFileBO.FileData).Result;
                }
                using (var _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>())
                {
                    try
                    {
                        if (patientEDFFileBO.FileData.Length > 252)
                        {
                            var noOfRecords = Convert.ToInt32(Encoding.ASCII.GetString(patientEDFFileBO.FileData.Skip(236).Take(8).ToArray()));
                            var durationPerRecord = Convert.ToInt32(Encoding.ASCII.GetString(patientEDFFileBO.FileData.Skip(244).Take(8).ToArray()));
                            var durationOfFile = noOfRecords * durationPerRecord;
                            if (durationOfFile > 0)
                                patientEDFFileBO.Duration = durationOfFile;
                        }
                        _patientEDFFileRepository.Insert(_mapper.Map<PatientEDFFile>(patientEDFFileBO));
                        _logger.LogInformation("Deleting the quick evaluation file");
                        DeleteQuickEvaluationFiles(patientEDFFileBO.PatientCaseID);
                        var patientcase = _patientCasesRepository.GetByID(patientEDFFileBO.PatientCaseID);
                        var patient = _patientRepository.GetByPatientCaseID(patientEDFFileBO.PatientCaseID);
                        if ((sessionContext.RoleCode == RoleCodes.Pharmacist || sessionContext.RoleCode == RoleCodes.Pharmacy)
                            && (patient.PharmacyID != sessionContext.PharmacyID))
                            throw new ServiceException(ClientSideErrors.USER_NOT_AUTHORIZED);
                        if (patientcase.StatusID == CaseStatus.DeviceReturned)
                        {
                            _logger.LogInformation("Updating patient case for returned devcie ");
                            patientcase.StatusID = CaseStatus.QuickEvalInQueue;
                            patientcase.UpdatedBy = sessionContext.LoginName;
                            _patientCasesRepository.Update(patientcase);
                        }
                        EdfFileUpdatePayLoadBO edfFileUpdatePayLoadBO = new EdfFileUpdatePayLoadBO
                        {
                            PatientBO = _mapper.Map<PatientBO>(patient),
                            SessionContext = GetSessionContext(),
                            CorrelationId = GetCorrelationId()
                        };
                        _rabbitMQClient.SendMessage(KnownChannels.NAVIGATOR_EDF_FILE_UPLOADED_EVENT_CHANNEL, edfFileUpdatePayLoadBO);
                        _unitOfWork.Commit();
                    }
                    catch (Exception ex)
                    {
                        _unitOfWork.Rollback();
                        throw ex.InnerException ?? ex;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }
        }
        public List<string> DownloadQuickEvalImages(long patientcaseid)
        {
            _logger.LogInformation($"DownloadQuickEvalImages: patient case id {patientcaseid}");
            List<string> imageBase64String = new List<string>();
            try
            {
                var zipfile = DownloadQuickEvalFile(patientcaseid);
                if (zipfile == null)
                {
                    _logger.LogWarning($"QuickEval file unable to download ");
                    return null;
                }
                    
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
                _logger.LogInformation("File sucessfully encoded in base64 string");
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
        public void DeleteDetailEvaluationFiles(long patientcaseid)
        {
            _logger.LogInformation($"DeleteDetailEvaluationFiles: patient case id {patientcaseid}");
            try
            {
                var evaluationFiles = _detailEvaluationRepository.GetByPatientCaseId(patientcaseid);
                foreach (var f in evaluationFiles)
                {
                    using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.FileSharingServiceBaseUrl, TimeSpan.FromMinutes(15)))
                    {
                        var res = apiClient.DeleteAsync($"api/v1/filesharing/{f.ResultsPath}").Result;
                        _detailEvaluationRepository.Delete(f);
                        _logger.LogInformation($"DeleteDetailEvaluationFiles: completed");

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }
        }

        public DetailEvaluationBO GetDetailEvaluationData(long patientcaseid)
        {
            _logger.LogInformation($"GetDetailEvaluationData: patient case id {patientcaseid}");
            try
            {
                var ecgFile = _detailEvaluationRepository.GetByPatientCaseId(patientcaseid).FirstOrDefault();
                return _mapper.Map<DetailEvaluationBO>(ecgFile);
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }
        }
        public DetailEvaluationBO UpdateDetailEvaluationData(DetailEvaluationBO detailEvaluationBO)
        {
            _logger.LogInformation($"UpdateDetailEvaluationData: patient case id {detailEvaluationBO.PatientCaseID}");
            try
            {
                var sessionContext = GetSessionContext();
                var patientCase = _patientCasesRepository.GetByID(detailEvaluationBO.PatientCaseID);
                if (patientCase == null)
                    throw new ServiceException(ClientSideErrors.INVALID_USER_ACTION);
                if(patientCase.CardiologistID != sessionContext.CardiologistID)
                    throw new ServiceException(ClientSideErrors.USER_NOT_AUTHORIZED);
                var ecgFiledb = _detailEvaluationRepository.GetByPatientCaseId(detailEvaluationBO.PatientCaseID).FirstOrDefault();
                if (ecgFiledb == null)
                    throw new ServiceException(ClientSideErrors.INVALID_USER_ACTION);
                ecgFiledb.NotesTypeID = detailEvaluationBO.NotesTypeID;
                ecgFiledb.Notes = detailEvaluationBO.Notes;
                _detailEvaluationRepository.Update(ecgFiledb);
                return _mapper.Map<DetailEvaluationBO>(ecgFiledb);
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }
        }
        public DetailEvaluationBO DownloadDetailEvaluationFile(long patientcaseid)
        {
            try
            {
                _logger.LogInformation($"DownloadDetailEvaluationFile: patient case id {patientcaseid}");

                var ecgFile = _detailEvaluationRepository.GetByPatientCaseId(patientcaseid).FirstOrDefault();
                DetailEvaluationBO ret = null;
                if (ecgFile != null)
                {
                    using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.FileSharingServiceBaseUrl, TimeSpan.FromMinutes(15)))
                    {
                        var data = apiClient.DownloadGetAsync($"api/v1/filesharing/{ecgFile.ResultsPath}", "application/x-www-form-urlencoded", "*/*").Result;
                        ret = _mapper.Map<DetailEvaluationBO>(ecgFile);
                        ret.FileData = data.Take((int)ret.FileLength).ToArray();
                    }
                }

                return ret;
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }
        }

        public void UploadDetailEvaluationFile(DetailEvaluationBO patientEcgFile)
        {
            using (var _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>())
            {
                try
                {
                    _logger.LogInformation($"Upload detail evaluation file start");
                    var sessionContext = GetSessionContext();
                    var patientcase = _patientCasesRepository.GetByID(patientEcgFile.PatientCaseID);
                    if (patientcase.CardiologistID != sessionContext.CardiologistID && (sessionContext.RoleCode == RoleCodes.Cardiologist || sessionContext.RoleCode == RoleCodes.Nurse))
                        throw new ServiceException(ClientSideErrors.INVALID_USER_ACTION);
                    if (sessionContext.RoleCode != RoleCodes.Cardiologist && sessionContext.RoleCode != RoleCodes.CentralGroupUser && sessionContext.RoleCode != RoleCodes.Nurse)
                        throw new ServiceException(ClientSideErrors.USER_NOT_AUTHORIZED);
                    DeleteDetailEvaluationFiles(patientEcgFile.PatientCaseID);
                    patientEcgFile.CreatedBy = sessionContext.LoginName;
                    using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.FileSharingServiceBaseUrl, TimeSpan.FromMinutes(15)))
                    {
                        var res = apiClient.PostFileAsync($"api/v1/filesharing/{patientEcgFile.ResultsPath}", Path.GetFileName(patientEcgFile.ResultsPath), patientEcgFile.FileData).Result;
                    }
                    _detailEvaluationRepository.Insert(_mapper.Map<DetailEvaluation>(patientEcgFile));
                    if(patientEcgFile.IsSigned)
                        patientcase.StatusID = CaseStatus.DetailEvalCompleted;
                    else
                        patientcase.StatusID = CaseStatus.DetailEvalUploaded;
                    patientcase.EndDate = DateTime.UtcNow;
                    patientcase.UpdatedBy = sessionContext.LoginName;
                    _patientCasesRepository.Update(patientcase);
                    _unitOfWork.Commit();

                    var p = _patientService.GetPatientbyCaseID(patientEcgFile.PatientCaseID);
                    
                    _rabbitMQClient.SendMessage(KnownChannels.DETAILED_EVALUATION_UPLOADED_COMPLETED_EVENT_CHANNEL,
                        new DECompletedEventPayloadBO()
                        {
                            SessionContext = sessionContext,
                            PatientBO = p
                        });
                    _patientService.RaiseNavigatorCleanEvent(p);
                    _logger.LogInformation($"Upload detail evaluation file completed");
                }
                catch (Exception ex)
                {
                    _unitOfWork.Rollback();
                    throw ex.InnerException ?? ex;
                }
            }
        }

        public DetailEvaluationBO GetPatientEvaluationByCaseID(long patientcaseid)
        {
            _logger.LogInformation($"GetPatientEvaluationByCaseID: patient case id {patientcaseid}");
            var evaluationFiles = _detailEvaluationRepository.GetByPatientCaseId(patientcaseid).FirstOrDefault();
            return _mapper.Map<DetailEvaluationBO>(evaluationFiles);
        }

        #endregion //EDF and Evaluation file
        #region Patient Quick Evaluation Result
        private void DeleteQuickEvaluationFiles(long patientcaseid)
        {
            try
            {
                _logger.LogInformation($"DeleteQuickEvaluationFiles: patient case id {patientcaseid}");
                var evaluationFiles = _quickEvaluationFileRepository.GetByPatientCaseId(patientcaseid);
                foreach (var f in evaluationFiles)
                {
                    using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.FileSharingServiceBaseUrl, TimeSpan.FromMinutes(15)))
                    {
                        var res = apiClient.DeleteAsync($"api/v1/filesharing/{f.FilePath}").Result;
                        _quickEvaluationFileRepository.Delete(f);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }
        }
        public QuickEvaluationFileBO GetQuickEvalFile(long patientcaseid)
        {
            try
            {
                _logger.LogInformation($"GetQuickEvalFile: patient case id {patientcaseid}");
                return _mapper.Map<QuickEvaluationFileBO>(_quickEvaluationFileRepository.GetByPatientCaseId(patientcaseid).FirstOrDefault());
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }
        }
        public QuickEvaluationFileBO DownloadQuickEvalFile(long patientcaseid)
        {
            _logger.LogInformation($"DownloadQuickEvalFile: patient case id {patientcaseid}");
            try
            {
                var ecgFile = _quickEvaluationFileRepository.GetByPatientCaseId(patientcaseid).FirstOrDefault();
                QuickEvaluationFileBO ret = null;
                if (ecgFile != null)
                {
                    using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.FileSharingServiceBaseUrl, TimeSpan.FromMinutes(15)))
                    {
                        var data = apiClient.DownloadGetAsync($"api/v1/filesharing/{ecgFile.FilePath}", "application/x-www-form-urlencoded", "*/*").Result;
                        ret = _mapper.Map<QuickEvaluationFileBO>(ecgFile);
                        ret.FileData = data.Take((int)ret.FileLength).ToArray();
                    }
                }
                return ret;
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }
        }

        public void UploadQuickEvalFile(QuickEvaluationFileBO quickEvalFileBO)
        {
            using (var _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>())
            {
                try
                {
                    _logger.LogInformation($"UploadQuickEvalFile: patient case id {quickEvalFileBO.PatientCaseID}");
                    var sessionContext = GetSessionContext();
                    DeleteQuickEvaluationFiles(quickEvalFileBO.PatientCaseID);
                    quickEvalFileBO.CreatedBy = sessionContext?.LoginName ?? SystemUsers.Navigator;
                    using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.FileSharingServiceBaseUrl, TimeSpan.FromMinutes(15)))
                    {
                        var res = apiClient.PostFileAsync($"api/v1/filesharing/{quickEvalFileBO.FilePath}", Path.GetFileName(quickEvalFileBO.FilePath), quickEvalFileBO.FileData).Result;
                    }
                    _quickEvaluationFileRepository.Insert(_mapper.Map<QuickEvaluationFile>(quickEvalFileBO));
                    _rabbitMQClient.SendMessage(KnownChannels.QUICK_EVALUATION_COMPLETED_EVENT_CHANNEL, _patientService.GetPatientbyCaseID(quickEvalFileBO.PatientCaseID));
                    _logger.LogInformation($"UploadQuickEvalFile: completed");
                    _unitOfWork.Commit();
                }
                catch (Exception ex)
                {
                    _unitOfWork.Rollback();
                    throw ex.InnerException ?? ex;
                }
            }
        }
        public QuickEvaluationResultBO GetQuickEvaluationResultByCaseID(long patientcaseid)
        {
            _logger.LogInformation($"GetQuickEvaluationResultByCaseID: patient case id {patientcaseid}");

            var sessionContext = GetSessionContext();
            var quickEvaluationResult = _quickEvaluationResultRepository.GetByPatientCaseId(patientcaseid);
            var patient = _patientRepository.GetByPatientCaseID(patientcaseid);
            if ((sessionContext.RoleCode == RoleCodes.Pharmacist || sessionContext.RoleCode == RoleCodes.Pharmacy)
                && (patient.PharmacyID != sessionContext.PharmacyID))
                throw new ServiceException(ClientSideErrors.USER_NOT_AUTHORIZED);

            if ((sessionContext.RoleCode == RoleCodes.CentralGroupUser || sessionContext.RoleCode == RoleCodes.Cardiologist || sessionContext.RoleCode == RoleCodes.Nurse) &&
                patient.StatusID < CaseStatus.QuickEvalCompleted)
                throw new ServiceException(ClientSideErrors.INVALID_USER_ACTION);
            return _mapper.Map<QuickEvaluationResultBO>(quickEvaluationResult);
        }
        public long AddQuickEvaluationResult(QuickEvaluationResultBO quickEvaluationResultBO)
        {
            using (var _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>())
            {
                try
                {

                    var sessionContext = GetSessionContext();
                    _logger.LogInformation($"AddQuickEvaluationResult: patient  {sessionContext.LoginName}");

                    var quickEvaluationResult = _mapper.Map<QuickEvaluationResult>(quickEvaluationResultBO);
                    quickEvaluationResult.CreatedBy = sessionContext.LoginName;
                    var patient = _patientRepository.GetByPatientCaseID(quickEvaluationResultBO.PatientCaseID);
                    if ((sessionContext.RoleCode == RoleCodes.Pharmacist || sessionContext.RoleCode == RoleCodes.Pharmacy)
                        && (patient.PharmacyID != sessionContext.PharmacyID))
                        throw new ServiceException(ClientSideErrors.USER_NOT_AUTHORIZED);
                    var patientcase = _patientCasesRepository.GetByID(quickEvaluationResultBO.PatientCaseID);
                    patientcase.StatusID = CaseStatus.QuickEvalCompleted;
                    patientcase.UpdatedBy = sessionContext?.LoginName ?? SystemUsers.Navigator;
                    var oldResult = _quickEvaluationResultRepository.GetByPatientCaseId(quickEvaluationResultBO.PatientCaseID);
                    if(oldResult != null)
                        _quickEvaluationResultRepository.Delete(oldResult);
                    _patientCasesRepository.Update(patientcase);
                    _logger.LogInformation($"AddQuickEvaluationResult: Result added");

                    _quickEvaluationResultRepository.Insert(quickEvaluationResult);
                    _unitOfWork.Commit();
                    _logger.LogInformation($"AddQuickEvaluationResult: Send message to {KnownChannels.QUICK_EVALUATION_RESULT_EVENT_CHANNEL}");

                    _rabbitMQClient.SendMessage(KnownChannels.QUICK_EVALUATION_RESULT_EVENT_CHANNEL,
                        new QEResultEventPayloadBO()
                        {
                            SessionContext = sessionContext,
                            PatientBO = _patientService.GetPatientbyCaseID(quickEvaluationResultBO.PatientCaseID)
                        });
                    return quickEvaluationResult.ID;
                }
                catch (Exception ex)
                {
                    _unitOfWork.Rollback();
                    throw ex.InnerException ?? ex;
                }
            }
        }
        public void UpdateQuickEvaluationResult(QuickEvaluationResultBO quickEvaluationResultBO)
        {
            var sessionContext = GetSessionContext();
            var patient = _patientRepository.GetByPatientCaseID(quickEvaluationResultBO.PatientCaseID);
            if ((sessionContext.RoleCode == RoleCodes.Pharmacist || sessionContext.RoleCode == RoleCodes.Pharmacy)
                && (patient.PharmacyID != sessionContext.PharmacyID))
                throw new ServiceException(ClientSideErrors.USER_NOT_AUTHORIZED);
            var quickEvaluationResult = _quickEvaluationResultRepository.GetByID(quickEvaluationResultBO.ID);
            quickEvaluationResult.MeasurementTime = quickEvaluationResultBO.MeasurementTime;
            quickEvaluationResult.QuickResultID = quickEvaluationResultBO.QuickResultID;
            quickEvaluationResult.Notes = quickEvaluationResultBO.Notes;
            quickEvaluationResult.UpdatedBy = sessionContext?.LoginName ?? SystemUsers.Navigator;
            _quickEvaluationResultRepository.Update(quickEvaluationResult);
        }

        #endregion //Patient Case Additional Info CRUD
    }
}
