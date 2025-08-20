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
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Navigator.Service
{
    public class PatientService : BaseService, IPatientService
    {
        private readonly ILogger<PatientService> _logger;
        private readonly RabbitMQClient _rabbitMQClient;
        private readonly WebServiceConfiguration _webApiConf;
        private readonly EdfFilePathConfiguration _edfFileConfiguration;
        private readonly NavigatorConfiguration _navigatorConfiguration;
        private int _edfFileLength;
        private readonly Stopwatch _stopwatch;
        private int _remainingTime;
        public PatientService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<PatientService>>();
            _rabbitMQClient = _serviceProvider.GetRequiredService<RabbitMQClient>();
            _webApiConf = serviceProvider.GetRequiredService<WebServiceConfiguration>();
            _edfFileConfiguration = serviceProvider.GetRequiredService<EdfFilePathConfiguration>();
            _navigatorConfiguration = serviceProvider.GetRequiredService<NavigatorConfiguration>();
            _stopwatch = new Stopwatch();
        }
        public void Execute(EdfFileUpdatePayLoadBO edfFileUpdatePayloadBo, int retryCount)
        {
            _stopwatch.Restart();
            var sessionId = edfFileUpdatePayloadBo.CorrelationId ?? Guid.NewGuid().ToString();
            _rabbitMQClient.SendMessage(KnownChannels.QUICK_EVALUATION_PROGRESS_CHANNEL, new QuickEvaluationProgressMessage()
            { PatientCaseId = edfFileUpdatePayloadBo.PatientBO.CaseID, Step = QuickEvaluationProgress.EVALUATION_STARTED });
            bool bStopThread = false;
            using (_logger.BeginScope($"@{{{LoggingConstants.CorrelationId}}}", sessionId))
            {
                _logger.LogInformation($"Retry Count Patient case id {edfFileUpdatePayloadBo.PatientBO.CaseID} is {retryCount}");
                _logger.LogInformation($"Starting execution of EDF Worker for Patient case id {edfFileUpdatePayloadBo.PatientBO.CaseID}");
                try
                {
                    var patient = edfFileUpdatePayloadBo.PatientBO;
                    var sessionContext = edfFileUpdatePayloadBo.SessionContext;
                    Thread.SetData(Thread.GetNamedDataSlot(LoggingConstants.CorrelationId), sessionId);
                    Thread.SetData(Thread.GetNamedDataSlot("NambayaSession"), sessionContext);

                    string edfFileName = _edfFileConfiguration.EdfFilePath + @"\" + patient.CaseID + ".edf";
                    if (!edfFileUpdatePayloadBo.IsCleanup)
                    {
                        _logger.LogInformation($"Downloading Edf file for Patient case id {edfFileUpdatePayloadBo.PatientBO.CaseID}");
                        DownloadEdfFile(patient.CaseID, edfFileName, sessionContext);
                        _logger.LogInformation($"Starting to launch automation process for Patient case id {edfFileUpdatePayloadBo.PatientBO.CaseID}");
                    }

                    using Process process = new Process
                    {
                        EnableRaisingEvents = false,
                        StartInfo = new ProcessStartInfo
                        {
                            //WorkingDirectory = _navigatorConfiguration.JrePath,
                            UseShellExecute = false,
                            FileName = "java",
                            Arguments = "-jar " + '"' + _navigatorConfiguration.NavigatorExecutableJarPath + "\""
                                                + " \"" + _navigatorConfiguration.NavigatorUserName + "\"" //navigator username
                                                + " \"" + _navigatorConfiguration.NavigatorPassword + "\"" //navigator password
                                                + " \"" + edfFileName + "\"" //edf file name
                                                + " \"" + patient.FirstName + "\"" //patient first name
                                                + " \"" + patient.LastName + "\"" //patient last name
                                                + " \"" + patient.DateOfBirth + "\"" //patient date of birth
                                                + " \"" + GetGenderCodeByID(patient.GenderID) + "\"" //patient gender
                                                + " \"" + _navigatorConfiguration.QuickEvaluationReportPath + "\""
                                                + " \"" + patient.CaseIDString + "\""
                                                + " \"" + CalculateFileReadDelay() + "\""
                                                + " \"" + (edfFileUpdatePayloadBo.IsCleanup ? "clean" : "retain") + "\"",
                            RedirectStandardOutput = true, //Set output of program to be written to process output stream
                            RedirectStandardError = true
                        }
                    };
                    _logger.LogInformation($"TestLeft process params : {process.StartInfo.WorkingDirectory} {process.StartInfo.FileName} {process.StartInfo.Arguments}");
                    Thread broadCastThread = new Thread(() =>
                    {
                        int sleepSecs = 5;
                        while (bStopThread == false && _remainingTime - sleepSecs > 0)
                        {
                            Thread.Sleep(TimeSpan.FromSeconds(sleepSecs));
                            _remainingTime -= sleepSecs;
                            _rabbitMQClient.SendMessage(KnownChannels.QUICK_EVALUATION_PROGRESS_CHANNEL,
                            new QuickEvaluationProgressMessage()
                            {
                                PatientCaseId = patient.CaseID,
                                Step = QuickEvaluationProgress.EVALUATION_TIMESTAMP,
                                RemainingTime = _remainingTime,
                                TotalTimeElapsed = (int)_stopwatch.Elapsed.TotalSeconds,
                            });
                        }
                    });
                    broadCastThread.Start();
                    if (process.Start())
                    {
                        _logger.LogInformation($"Process Launched for Patient case id {edfFileUpdatePayloadBo.PatientBO.CaseID}");
                        _logger.LogError($"Automation Process errors for case id {edfFileUpdatePayloadBo.PatientBO.CaseID} {process.StandardError.ReadToEnd()}");
                        _logger.LogInformation($"Automation Process output for case id {edfFileUpdatePayloadBo.PatientBO.CaseID} {process.StandardOutput.ReadToEnd()}");
                        process.WaitForExit();
                        bStopThread = true;
                        broadCastThread.Join();
                        if (edfFileUpdatePayloadBo.IsCleanup)
                            CleanupServiceFiles(patient.CaseID);
                        else
                        {
                            UploadQuickEvaluationFile(patient.CaseID, sessionContext);
                            _rabbitMQClient.SendMessage(KnownChannels.QUICK_EVALUATION_COMPLETED_EVENT_CHANNEL, patient);
                        }
                    }
                }
                catch (ServiceException se)
                {
                    _stopwatch.Stop();
                    throw se;
                }
                catch (Exception ex)
                {
                    _stopwatch.Stop();
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
            try
            {
                _rabbitMQClient.SendMessage(KnownChannels.QUICK_EVALUATION_PROGRESS_CHANNEL,
                    new QuickEvaluationProgressMessage()
                    {
                        PatientCaseId = patientcaseid,
                        Step = QuickEvaluationProgress.EDF_FILE_DOWNLOADING,
                        TotalTimeElapsed = (int)_stopwatch.Elapsed.TotalSeconds
                    });
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
                this._remainingTime = CalculateProcessTime(patientcaseid);
                _rabbitMQClient.SendMessage(KnownChannels.QUICK_EVALUATION_PROGRESS_CHANNEL,
                    new QuickEvaluationProgressMessage()
                    {
                        PatientCaseId = patientcaseid,
                        Step = QuickEvaluationProgress.EDF_FILE_DOWNLOADED,
                        RemainingTime = _remainingTime,
                        TotalTimeElapsed = (int)_stopwatch.Elapsed.TotalSeconds,
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Edf download failed {patientcaseid} with error {ex.InnerException?.Message ?? ex.Message}");
                _rabbitMQClient.SendMessage(KnownChannels.QUICK_EVALUATION_PROGRESS_CHANNEL,
                    new QuickEvaluationProgressMessage()
                    {
                        PatientCaseId = patientcaseid,
                        Step = QuickEvaluationProgress.EDF_FILE_DOWNLOAD_FAILED,
                        TotalTimeElapsed = (int)_stopwatch.Elapsed.TotalSeconds
                    });
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        private void UploadQuickEvaluationFile(long patientcaseid, SessionContext sessionContext)
        {
            try
            {
                //TODO: get file from directory and upload it
                string strPatientCaseID = patientcaseid.ToString("D6");
                var zipFileName = _navigatorConfiguration.QuickEvaluationReportPath + @"\" + strPatientCaseID + ".zip";
                var qtcFileName = $"{_navigatorConfiguration.QuickEvaluationReportPath}\\{strPatientCaseID}_QTC.png";
                List<string> pngFileNames = new List<string>
                {
                    qtcFileName
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
                if (File.Exists(qtcFileName))
                    File.Delete(qtcFileName);
                _logger.LogInformation($"Upload Quick Eval zipfile succeeded for Patient case id {strPatientCaseID}");
                _rabbitMQClient.SendMessage(KnownChannels.QUICK_EVALUATION_PROGRESS_CHANNEL,
                    new QuickEvaluationProgressMessage()
                    {
                        PatientCaseId = patientcaseid,
                        Step = QuickEvaluationProgress.EVALUATION_FINISHED,
                        TotalTimeElapsed = (int)_stopwatch.Elapsed.TotalSeconds
                    });
            }
            catch (Exception ex)
            {
                _rabbitMQClient.SendMessage(KnownChannels.QUICK_EVALUATION_PROGRESS_CHANNEL,
                    new QuickEvaluationProgressMessage()
                    {
                        PatientCaseId = patientcaseid,
                        Step = QuickEvaluationProgress.EVALUATION_FAILED,
                        TotalTimeElapsed = (int)_stopwatch.Elapsed.TotalSeconds
                    });
                _logger.LogError(ex, $"Upload quick evaluation failed {patientcaseid} with error {ex.InnerException?.Message ?? ex.Message}");
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
        private int CalculateProcessTime(long patientcaseid)
        {
            int timeinSecs = CalculateFileReadDelay();
            var zipFileName = _navigatorConfiguration.QuickEvaluationReportPath + @"\" + patientcaseid.ToString("D6") + ".zip";
            if (File.Exists(zipFileName))
                timeinSecs += 10; // add 10 secs for cleanup
            // Add time for processing  the case
            timeinSecs += 60;
            return timeinSecs;
        }
        private void CleanupServiceFiles(long patientcaseid)
        {
            try
            {
                string strPatientCaseID = patientcaseid.ToString("D6");
                //TODO: get file from directory and upload it
                _logger.LogInformation($"Cleaning up files for Patient case id {patientcaseid}");
                string edfFileName = _edfFileConfiguration.EdfFilePath + @"\" + strPatientCaseID + ".edf";
                var zipFileName = _navigatorConfiguration.QuickEvaluationReportPath + @"\" + strPatientCaseID + ".zip";
                string imgFile = $"{_navigatorConfiguration.QuickEvaluationReportPath}\\{strPatientCaseID}_QTC.png";
                if (File.Exists(imgFile))
                    File.Delete(imgFile);
                if (File.Exists(zipFileName))
                    File.Delete(zipFileName);
                if (File.Exists(edfFileName))
                    File.Delete(edfFileName);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Cleaning up files for Patient case id failed {patientcaseid}");
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
    }
}