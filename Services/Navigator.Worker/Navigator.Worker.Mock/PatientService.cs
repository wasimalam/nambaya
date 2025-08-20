using Common.BusinessObjects;
using Common.Infrastructure;
using Common.Infrastructure.Exceptions;
using Common.Infrastructure.Helpers;
using Common.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Navigator.Contracts.Interfaces;
using Patient.Contracts.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Navigator.Worker.Mock
{
    public class PatientService : BaseService, IPatientService
    {
        private readonly ILogger<PatientService> _logger;
        private readonly RabbitMQClient _rabbitMQClient;
        private readonly WebServiceConfiguration _webApiConf;
        private readonly EdfFilePathConfiguration _edfFileConfiguration;
        private readonly NavigatorConfiguration _navigatorConfiguration;
        private int _edfFileLength;
        private readonly System.Diagnostics.Stopwatch _stopwatch;
        public PatientService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<PatientService>>();
            _rabbitMQClient = _serviceProvider.GetRequiredService<RabbitMQClient>();
            _webApiConf = serviceProvider.GetRequiredService<WebServiceConfiguration>();
            _edfFileConfiguration = serviceProvider.GetRequiredService<EdfFilePathConfiguration>();
            _navigatorConfiguration = serviceProvider.GetRequiredService<NavigatorConfiguration>();
            _stopwatch = new System.Diagnostics.Stopwatch();
        }
        public void Execute(EdfFileUpdatePayLoadBO edfFileUpdatePayloadBo, int retryCount)
        {
            _stopwatch.Restart();
            _logger.LogInformation($"Retry Count Patient case id {edfFileUpdatePayloadBo.PatientBO.CaseID} is {retryCount}");
            var sessionId = edfFileUpdatePayloadBo.CorrelationId ?? Guid.NewGuid().ToString();
            using (_logger.BeginScope($"@{{{LoggingConstants.CorrelationId}}}", sessionId))
            {
                _logger.LogInformation($"Starting execution of EDF Worker for Patient case id {edfFileUpdatePayloadBo.PatientBO.CaseID}");
                try
                {
                    var patient = edfFileUpdatePayloadBo.PatientBO;
                    var sessionContext = edfFileUpdatePayloadBo.SessionContext;
                    Thread.SetData(Thread.GetNamedDataSlot(LoggingConstants.CorrelationId), sessionId);
                    Thread.SetData(Thread.GetNamedDataSlot("NambayaSession"), sessionContext);

                    if (!edfFileUpdatePayloadBo.IsCleanup)
                    {
                        string edfFileName = _edfFileConfiguration.EdfFilePath + @"/" + patient.CaseID + ".edf";
                        _logger.LogInformation($"Downloading Edf file for Patient case id {edfFileUpdatePayloadBo.PatientBO.CaseID}");
                        DownloadEdfFile(patient.CaseID, edfFileName, sessionContext);
                        _logger.LogInformation($"Starting to launch automation process for Patient case id {edfFileUpdatePayloadBo.PatientBO.CaseID}");
                        File.Copy("Dummy.png", $"{_navigatorConfiguration.QuickEvaluationReportPath}/{edfFileUpdatePayloadBo.PatientBO.CaseID}_QTC.png");

                        _logger.LogInformation($"Process Launched for Patient case id {edfFileUpdatePayloadBo.PatientBO.CaseID}");
                        UploadQuickEvaluationFile(patient.CaseID, sessionContext);
                        _rabbitMQClient.SendMessage(KnownChannels.QUICK_EVALUATION_COMPLETED_EVENT_CHANNEL, patient);
                    }
                    else
                        CleanupServiceFiles(patient.CaseID);
                }
                catch (ServiceException se)
                {
                    throw se;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Exception occured for Patient Case id {edfFileUpdatePayloadBo.PatientBO.CaseID}", ex);
                    if (ex.InnerException != null)
                        throw ex.InnerException;
                    throw;
                }
            }
        }

        private string GetGenderCodeByID(long genderID)
        {
            if (genderID == 401)
            {
                return "M";
            }
            else if (genderID == 402)
            {
                return "W";
            }
            else if (genderID == 403)
            {
                return "X";
            }
            else if (genderID == 404)
            {
                return "D";
            }
            return "D";
        }

        private int CalculateFileReadDelay()
        {
            return _edfFileLength / 950 / 2048;
        }

        private void DownloadEdfFile(long patientcaseid, string edfFileName, SessionContext sessionContext)
        {
            var progress = new QuickEvaluationProgressMessage() { PatientCaseId = patientcaseid, Step = QuickEvaluationProgress.EDF_FILE_DOWNLOADING };
            try
            {
                progress.TotalTimeElapsed = (int)_stopwatch.Elapsed.TotalSeconds;
                _rabbitMQClient.SendMessage(KnownChannels.QUICK_EVALUATION_PROGRESS_CHANNEL, progress);
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PharmacistServiceBaseUrl, TimeSpan.FromMinutes(30)))
                {
                    _logger.LogInformation($"Starting Edf download for Patient case id {patientcaseid}");
                    var res = apiClient.DownloadGetAsync($"api/v1/patient/downloadedffile?patientcaseid={patientcaseid}").Result;
                    //_logger.LogInformation($"Decompress Edf for Patient case id {patientcaseid}");
                    //var decompressedBytes = CompressionHelper.Decompress(res);
                    _logger.LogInformation($"Saving Edf file for Patient case id {patientcaseid} bytes length {res.Length}");
                    _edfFileLength = res.Length;
                    File.WriteAllBytes(edfFileName, res);
                }
                progress.Step = QuickEvaluationProgress.EDF_FILE_DOWNLOADED;
                progress.TotalTimeElapsed = (int)_stopwatch.Elapsed.TotalSeconds;
                progress.RemainingTime = 10;
                _rabbitMQClient.SendMessage(KnownChannels.QUICK_EVALUATION_PROGRESS_CHANNEL, progress);
            }
            catch (Exception ex)
            {
                progress.Step = QuickEvaluationProgress.EDF_FILE_DOWNLOAD_FAILED;
                progress.TotalTimeElapsed = (int)_stopwatch.Elapsed.TotalSeconds;
                _rabbitMQClient.SendMessage(KnownChannels.QUICK_EVALUATION_PROGRESS_CHANNEL,  progress);
                _logger.LogError(ex, $"Edf download failed {patientcaseid} with error {ex.InnerException?.Message ?? ex.Message}");
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        private void UploadQuickEvaluationFile(long patientcaseid, SessionContext sessionContext)
        {
            var progress = new QuickEvaluationProgressMessage() { PatientCaseId = patientcaseid, Step = QuickEvaluationProgress.EVALUATION_FINISHED };
            try
            {
                //TODO: get file from directory and upload it
                var zipFileName = _navigatorConfiguration.QuickEvaluationReportPath + @"/" + patientcaseid + ".zip";
                List<string> pngFileNames = new List<string>
                {
                    $"{_navigatorConfiguration.QuickEvaluationReportPath}/{patientcaseid}_QTC.png"
                };
                if (File.Exists(zipFileName))
                    File.Delete(zipFileName);
                _logger.LogInformation($"Uploading Quick Eval zipfile for Patient case id {patientcaseid}");
                CompressionHelper.CreateZipFile(zipFileName, pngFileNames);
                var bytearrray = File.ReadAllBytes(zipFileName);
                //TODO: test file is read properly
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PharmacistServiceBaseUrl, TimeSpan.FromMinutes(15)))
                {
                    var res = apiClient.PostFileAsync($"api/v1/patient/uploadquickevalfile?patientcaseid={patientcaseid}",
                        zipFileName, bytearrray, $"application/zip").Result;
                }
                if (File.Exists($"{_navigatorConfiguration.QuickEvaluationReportPath}/{patientcaseid}_QTC.png"))
                    File.Delete($"{_navigatorConfiguration.QuickEvaluationReportPath}/{patientcaseid}_QTC.png");
                _logger.LogInformation($"Upload Quick Eval zipfile succeeded for Patient case id {patientcaseid}");
                progress.TotalTimeElapsed = (int)_stopwatch.Elapsed.TotalSeconds;
                _rabbitMQClient.SendMessage(KnownChannels.QUICK_EVALUATION_PROGRESS_CHANNEL, progress);
            }
            catch (Exception ex)
            {
                progress.Step = QuickEvaluationProgress.EVALUATION_FAILED;
                progress.TotalTimeElapsed = (int)_stopwatch.Elapsed.TotalSeconds;
                _rabbitMQClient.SendMessage(KnownChannels.QUICK_EVALUATION_PROGRESS_CHANNEL,  progress);
                _logger.LogError(ex, $"Upload quick evaluation failed {patientcaseid} with error {ex.InnerException?.Message ?? ex.Message}");
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        private void CleanupServiceFiles(long patientcaseid)
        {
            try
            {
                //TODO: get file from directory and upload it
                _logger.LogInformation($"Cleaning up files for Patient case id {patientcaseid}");
                string edfFileName = _edfFileConfiguration.EdfFilePath + @"/" + patientcaseid + ".edf";
                var zipFileName = _navigatorConfiguration.QuickEvaluationReportPath + @"/" + patientcaseid + ".zip";
                string imgFile = $"{_navigatorConfiguration.QuickEvaluationReportPath}/{patientcaseid}_QTC.png";
                if (File.Exists(imgFile))
                    File.Delete(imgFile);
                if (File.Exists(zipFileName))
                    File.Delete(zipFileName);
                if (File.Exists(edfFileName))
                    File.Delete(edfFileName);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Cleaning up files for Patient case id failed {patientcaseid}  with error {ex.InnerException?.Message ?? ex.Message}");
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
    }
}