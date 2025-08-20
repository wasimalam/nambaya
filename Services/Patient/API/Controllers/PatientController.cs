using Common.Infrastructure;
using Common.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Patient.Contracts.Interfaces;
using Patient.Contracts.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;

namespace Patient.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    //[Authorize]
    //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class PatientController : ControllerBase
    {
        private readonly ILogger<PatientController> _logger;
        private readonly IPatientService _patientService;
        private readonly ICaseNotesService _caseNotesService;
        private readonly IDrugService _drugService;
        private readonly IAdditionalInfoService _additionalInfoService;
        private readonly IPatientImportService _patientImportService;
        private readonly ICaseDispatchService _caseDispatchService;
        private readonly IEvaluationService _evaluationService;
        private readonly IDashboardService _dashboardService;

        public PatientController(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<PatientController>>();
            _patientService = serviceProvider.GetRequiredService<IPatientService>();
            _drugService = serviceProvider.GetRequiredService<IDrugService>();
            _patientImportService = serviceProvider.GetRequiredService<IPatientImportService>();
            _caseNotesService = serviceProvider.GetRequiredService<ICaseNotesService>();
            _additionalInfoService = serviceProvider.GetRequiredService<IAdditionalInfoService>();
            _caseDispatchService = serviceProvider.GetRequiredService<ICaseDispatchService>();
            _evaluationService = serviceProvider.GetRequiredService<IEvaluationService>();
            _dashboardService = serviceProvider.GetRequiredService<IDashboardService>();
        }
        #region patient and cases 
        [Authorize("all")]
        [HttpGet]
        public ActionResult<PagedResults<PatientBO>> Get(int offset = 0, int limit = 0, string orderby = null, string filter = null)
        {
            var pat = _patientService.GetPatientCases(limit, offset, orderby, filter);
            if (pat == null)
            {
                return NotFound();
            }
            return Ok(pat);
        }

        [Authorize("all")]
        [HttpGet("cases/todispatch")]
        public ActionResult<PagedResults<PatientBO>> GetCasesToDispatched(int offset = 0, int limit = 0, string orderby = null, string filter = null)
        {
            var pat = _patientService.GetCasesToDispatch(limit, offset, orderby, filter);
            if (pat == null)
            {
                return NotFound();
            }
            return Ok(pat);
        }

        [Authorize("all")]
        [HttpGet("{id}")]
        public ActionResult<PatientBO> Get(long id)
        {
            var pat = _patientService.GetPatientbyID(id);
            if (pat == null)
            {
                return NotFound();
            }
            return Ok(pat);
        }
        [Authorize("all")]
        [HttpGet("cases/{patientcaseid}")]
        public ActionResult<PatientBO> GetPatientbyCaseID(long patientcaseid)
        {
            var pat = _patientService.GetPatientbyCaseID(patientcaseid);
            if (pat == null)
            {
                return NotFound();
            }
            return Ok(pat);
        }
        [Authorize("all")]
        [HttpGet("{action}/{patientcaseid}")]
        public ActionResult<PatientCaseBO> GetPatientCase(long patientcaseid)
        {
            var pat = _patientService.GetPatientCase(patientcaseid);
            if (pat == null)
            {
                return NotFound();
            }

            return Ok(pat);
        }
        [Authorize("all")]
        [HttpGet("{action}/{patientid}")]
        public ActionResult<List<PatientCaseBO>> GetPatientCases(long patientid)
        {
            var pat = _patientService.GetPatientCasesByPatient(patientid);
            return Ok(pat);
        }
        [Authorize("all")]
        [HttpPut("cases")]
        public ActionResult<PatientCaseBO> UdpatePatientCase(PatientCaseBO patientCaseBO)
        {
            var p = _patientService.UpdatePatientCase(patientCaseBO);
            return Ok(p);
        }
        [Authorize("all")]
        [HttpPut("{action}/{patientId}")]
        public ActionResult<PatientBO> DeActivatePatient(long patientId)
        {
            try
            {
                var p = _patientService.DeActivatePatient(patientId);
                if (p == null)
                    return NotFound();
                return Ok(p);
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return UnprocessableEntity(ex.Message);
            }
        }
        [Authorize("all")]
        [HttpDelete()]
        public ActionResult Delete(PatientBO patientBO)
        {
            try
            {
                _patientService.DeletePatient(patientBO);
                return Ok();
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return UnprocessableEntity(ex.Message);
            }
        }
        [Authorize("all")]
        [HttpGet("cases/evaluations/{patientcaseid}")]
        public ActionResult<DetailEvaluationBO> GetEvaluation(long patientcaseid)
        {
            var pat = _evaluationService.GetPatientEvaluationByCaseID(patientcaseid);
            if (pat == null)
            {
                return NotFound();
            }

            return Ok(pat);
        }

        [Authorize("all")]
        [HttpPost("{action}"), DisableRequestSizeLimit]
        public ActionResult<PatientBO> ImportPatientFile(string pharmacypatientid, long pharmacyid, string patientEmail, string patientPhone)
        {
            _logger.LogTrace($"ImportPatientFile Enter {pharmacypatientid}");
            try
            {
                var file = Request.Form.Files[0];
                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    //var fullPath = $"EDF/{patientcaseid}/{System.Guid.NewGuid().ToString()}";
                    using (var stream = new MemoryStream())
                    {
                        file.CopyTo(stream);
                        var bytes = stream.GetBuffer();
                        //string fileXmlContent = Encoding.UTF8.GetString(bytes);
                        return Ok(_patientImportService.ImportXml(bytes, pharmacypatientid, patientEmail, patientPhone, fileName, file.ContentType));
                    }
                }
                else
                    return BadRequest(ClientSideErrors.INVALID_PATIENT_IMPORT_DATA);
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return UnprocessableEntity(ex.Message);
            }
        }

        [Authorize("patientimport")]
        [HttpPost("{action}"), DisableRequestSizeLimit]
        public ActionResult ImportXml(string pharmacypatientid, string patientEmail, string patientPhone, string filename, string fileContentType, [FromBody] byte[] fileContent)
        {
            try
            {
                var pat = _patientImportService.ImportXml(fileContent, pharmacypatientid, patientEmail, patientPhone, filename, fileContentType);
                return Ok(pat);
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return UnprocessableEntity(ex.Message);
            }
        }

        [Authorize("all")]
        [HttpPost]
        public ActionResult<PatientBO> AddPatient(PatientBO patient)
        {
            try
            {
                var id = _patientService.AddPatient(patient);
                return _patientService.GetPatientbyCaseID(patient.CaseID);
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return UnprocessableEntity(ex.Message);
            }
        }

        [Authorize("all")]
        [HttpPut]
        public ActionResult<PatientBO> UpdatePatient(PatientBO patient)
        {
            try
            {
                var ph = _patientService.GetPatientbyID(patient.ID);
                if (ph == null)
                {
                    return NotFound();
                }
                _patientService.UpdatePatient(patient);
                if (patient.CaseID != 0)
                    return _patientService.GetPatientbyCaseID(patient.CaseID);
                return _patientService.GetPatientbyID(patient.ID);
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return UnprocessableEntity(ex.Message);
            }
        }

        #endregion
        [Authorize("all")]
        [HttpPut("{action}")]
        public ActionResult<PatientBO> AssignPatientCase(long patientcaseid)
        {
            try
            {
                var patient = _patientService.AssignPatientCase(patientcaseid);
                if (patient == null)
                    return NotFound();
                return Ok(patient);
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return UnprocessableEntity(ex.Message);
            }
        }
        [Authorize("all")]
        [HttpGet("{action}")]
        public ActionResult<PatientEDFFileBO> GetPatientEDFFile(long patientcaseid)
        {
            try
            {
                PatientEDFFileBO edf = _evaluationService.GetPatientEDFFile(patientcaseid);
                if (edf == null)
                    return NotFound();
                return Ok(edf);
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return UnprocessableEntity(ex.Message);
            }
        }
        [Authorize("all")]
        [HttpGet("{action}"), DisableRequestSizeLimit]
        public ActionResult DownloadEDFFile(long patientcaseid)
        {
            PatientEDFFileBO filedata;
            try
            {
                filedata = _evaluationService.DownloadEDFFile(patientcaseid);
                if (filedata != null && filedata.FileData != null)
                    return File(filedata.FileData, filedata.ContentType, filedata.FileName);
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return UnprocessableEntity(ex.Message);
            }

            return NotFound();
        }
        [Authorize("all")]
        [HttpPost("{action}"), DisableRequestSizeLimit, RequestFormLimits(MultipartBodyLengthLimit = System.Int32.MaxValue, ValueLengthLimit = System.Int32.MaxValue)]
        public ActionResult UploadEDFFile(long patientcaseid)
        {
            _logger.LogTrace($"UploadEDFFile Enter {patientcaseid}");
            try
            {
                var file = Request.Form.Files[0];
                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fullPath = $"EDF/{patientcaseid}/{System.Guid.NewGuid().ToString()}";
                    using (var stream = new MemoryStream())
                    {
                        file.CopyTo(stream);
                        _evaluationService.UploadEDFFile(
                            new PatientEDFFileBO
                            {
                                PatientCaseID = patientcaseid,
                                FilePath = fullPath,
                                FileName = fileName,
                                FileLength = file.Length,
                                FileData = stream.GetBuffer(),
                                ContentType = file.ContentType??$"application/octet-stream"
                            }
                            );
                    }
                    return Ok();
                }
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return UnprocessableEntity(ex.Message);
            }

            return BadRequest(ClientSideErrors.FAILED_TO_UPLOAD);
        }
        [Authorize("all")]
        [HttpPost("{action}"), DisableRequestSizeLimit, RequestFormLimits(MultipartBodyLengthLimit = System.Int32.MaxValue, ValueLengthLimit = System.Int32.MaxValue)]
        public ActionResult UploadEDFFileData([FromForm(Name = "")] PatientEDFFileBO patientEDFFileBO)
        {
            try
            {
                var file = Request.Form.Files[0];
                if (file.Length > 0)
                {
                    //var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    patientEDFFileBO = System.Text.Json.JsonSerializer.Deserialize<PatientEDFFileBO>(Request.Form[Request.Form.Keys.FirstOrDefault()]);
                    //var fullPath = $"EDF/{patientEDFFileBO.PatientCaseID}/{System.Guid.NewGuid().ToString()}";
                    using (var stream = new MemoryStream())
                    {
                        file.CopyTo(stream);
                        patientEDFFileBO.FileData = stream.GetBuffer();
                        _evaluationService.UploadEDFFile(patientEDFFileBO);
                    }
                    return Ok();
                }
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return UnprocessableEntity(ex.Message);
            }

            return BadRequest(ClientSideErrors.FAILED_TO_UPLOAD);
        }
        [Authorize("all")]
        [HttpGet("{action}")]
        public ActionResult<QuickEvaluationFileBO> GetQuickEvaluationFile(long patientcaseid)
        {
            try
            {
                QuickEvaluationFileBO qev = _evaluationService.GetQuickEvalFile(patientcaseid);
                if (qev == null)
                    return NotFound();
                return Ok(qev);
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return UnprocessableEntity(ex.Message);
            }
        }
        [Authorize("all")]
        [HttpGet("{action}"), DisableRequestSizeLimit]
        public ActionResult DownloadQuickEvaluationFile(long patientcaseid)
        {
            QuickEvaluationFileBO filedata;
            try
            {
                filedata = _evaluationService.DownloadQuickEvalFile(patientcaseid);
                if (filedata != null && filedata.FileData != null)
                    return File(filedata.FileData, filedata.ContentType, filedata.FileName);
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return UnprocessableEntity(ex.Message);
            }

            return NotFound();
        }
        [Authorize("all")]
        [HttpGet("{action}")]
        public ActionResult DownloadQuickEvaluationImages(long patientcaseid)
        {
            try
            {
                List<string> imageBytes;
                imageBytes = _evaluationService.DownloadQuickEvalImages(patientcaseid);
                if (imageBytes == null)
                    return NotFound();
                return Ok(
                    new
                    {
                        patientCaseid = patientcaseid,
                        data = imageBytes,
                        length = imageBytes.Count
                    }
                );
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return UnprocessableEntity(ex.Message);
            }
        }
        [Authorize("all")]
        [HttpGet("{action}"), DisableRequestSizeLimit]
        public ActionResult<QuickEvaluationFileBO> DownloadQuickEvaluationFileData(long patientcaseid)
        {
            QuickEvaluationFileBO filedata;
            try
            {
                filedata = _evaluationService.DownloadQuickEvalFile(patientcaseid);
                if (filedata == null)
                    return NotFound();
                return Ok(filedata);
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return UnprocessableEntity(ex.Message);
            }
        }
        [Authorize("all")]
        [HttpPost("{action}"), DisableRequestSizeLimit]
        public ActionResult UploadQuickEvaluationFile(long patientcaseid)
        {
            _logger.LogTrace($"UploadQuickEvaluationFile Enter {patientcaseid}");
            try
            {
                var file = Request.Form.Files[0];
                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fullPath = $"QE/{patientcaseid}/{System.Guid.NewGuid().ToString()}";
                    using (var stream = new MemoryStream())
                    {
                        file.CopyTo(stream);
                        _evaluationService.UploadQuickEvalFile(
                            new QuickEvaluationFileBO
                            {
                                PatientCaseID = patientcaseid,
                                FilePath = fullPath,
                                FileName = fileName,
                                FileLength = file.Length,
                                FileData = stream.GetBuffer(),
                                ContentType = file.ContentType ?? $"application/octet-stream",
                            });
                    }
                    return Ok();
                }
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return UnprocessableEntity(ex.Message);
            }

            return BadRequest(ClientSideErrors.FAILED_TO_UPLOAD);
        }
        [Authorize("all")]
        [HttpPost("{action}"), DisableRequestSizeLimit, RequestFormLimits(MultipartBodyLengthLimit = System.Int32.MaxValue, ValueLengthLimit = System.Int32.MaxValue)]
        public ActionResult UploadQuickEvaluationFileData([FromForm] QuickEvaluationFileBO quickEvaluationFileBO)
        {
            try
            {
                var file = Request.Form.Files[0];
                if (file.Length > 0)
                {
                    quickEvaluationFileBO = System.Text.Json.JsonSerializer.Deserialize<QuickEvaluationFileBO>(Request.Form[Request.Form.Keys.FirstOrDefault()]);
                    using (var stream = new MemoryStream())
                    {
                        file.CopyTo(stream);
                        quickEvaluationFileBO.FileData = stream.GetBuffer();
                        _evaluationService.UploadQuickEvalFile(quickEvaluationFileBO);
                    }
                    return Ok();
                }
                return BadRequest(ClientSideErrors.INVALID_DATA_TO_UPLOAD);
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (System.Exception ex)
            {
                return UnprocessableEntity(ex.Message);
            }
        }
        [Authorize("all")]
        [HttpGet("{action}")]
        public ActionResult GetECGReportData(long patientcaseid)
        {
            DetailEvaluationBO databo;
            try
            {
                databo = _evaluationService.GetDetailEvaluationData(patientcaseid);
                if (databo != null)
                    return Ok(databo);
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return UnprocessableEntity(ex.Message);
            }

            return NotFound();
        }
        [Authorize("all")]
        [HttpPut("{action}")]
        public ActionResult UpdateECGReportData(DetailEvaluationBO detailEvaluationBO)
        {
            DetailEvaluationBO databo;
            try
            {
                databo = _evaluationService.UpdateDetailEvaluationData(detailEvaluationBO);
                if (databo != null)
                    return Ok(databo);
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return UnprocessableEntity(ex.Message);
            }
            return NotFound();
        }
        [Authorize("all")]
        [HttpGet("{action}")]
        public ActionResult DownloadECGFile(long patientcaseid)
        {
            DetailEvaluationBO filedata;
            try
            {
                filedata = _evaluationService.DownloadDetailEvaluationFile(patientcaseid);
                if (filedata != null && filedata.FileData != null)
                    return File(filedata.FileData, filedata.ContentType, filedata.FileName);
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return UnprocessableEntity(ex.Message);
            }

            return NotFound();
        }
        [Authorize("all")]
        [HttpPost("{action}"), DisableRequestSizeLimit, RequestFormLimits(MultipartBodyLengthLimit = System.Int32.MaxValue, ValueLengthLimit = System.Int32.MaxValue)]
        public ActionResult UploadECGFile(long patientcaseid)
        {
            _logger.LogTrace($"UploadECGFile Enter {patientcaseid}");
            try
            {
                var file = Request.Form.Files[0];
                string notes = Request.Form["CaseNotes"];
                string notesttypeid = Request.Form["NotesTypeId"];
                long notesTypeID = 0;
                long.TryParse(notesttypeid, out notesTypeID);
                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fullPath = $"ECG/{patientcaseid}/{System.Guid.NewGuid().ToString()}";
                    using (var stream = new MemoryStream())
                    {
                        file.CopyTo(stream);
                        _evaluationService.UploadDetailEvaluationFile(
                            new DetailEvaluationBO
                            {
                                PatientCaseID = patientcaseid,
                                ResultsPath = fullPath,
                                FileName = fileName,
                                FileLength = file.Length,
                                ContentType = file.ContentType ?? $"application/octet-stream",
                                NotesTypeID = (long?)(notesTypeID == 0 ? ((long?)null) : notesTypeID),
                                Notes = notes,
                                FileData = stream.GetBuffer()
                            }
                            );
                    }

                    return Ok();
                }
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (System.Exception ex)
            {
                return UnprocessableEntity(ex.Message);
            }

            return BadRequest(ClientSideErrors.FAILED_TO_UPLOAD);
        }
        [Authorize("all")]
        [HttpPost("{action}"), DisableRequestSizeLimit]
        public ActionResult UploadECGFileData([FromForm] DetailEvaluationBO detailEvaluationBO)
        {
            try
            {
                var file = Request.Form.Files[0];
                if (file.Length > 0)
                {
                    detailEvaluationBO = System.Text.Json.JsonSerializer.Deserialize<DetailEvaluationBO>(Request.Form[Request.Form.Keys.FirstOrDefault()]);
                    using (var stream = new MemoryStream())
                    {
                        file.CopyTo(stream);
                        detailEvaluationBO.FileData = stream.GetBuffer();
                        _evaluationService.UploadDetailEvaluationFile(detailEvaluationBO);
                    }
                    return Ok();
                }
                return BadRequest(ClientSideErrors.INVALID_DATA_TO_UPLOAD);
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (System.Exception ex)
            {
                return UnprocessableEntity(ex.Message);
            }
        }
        [Authorize("all")]
        [HttpGet("{action}/{deviceid}")]
        public ActionResult<PatientBO> GetPatientByAssignedDevice(long deviceid)
        {
            try
            {
                var obj = _patientService.GetPatientByAssignedDevice(deviceid);
                if (obj == null)
                    return NotFound();
                return Ok(obj);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }
        #region Device
        [Authorize("all")]
        [HttpGet("{action}/{id}")]
        public ActionResult<DeviceAssignmentBO> GetDeviceAssignmentById(long id)
        {
            try
            {
                var obj = _patientService.GetDeviceAssignmentById(id);
                if (obj == null)
                    return NotFound();
                return Ok(obj);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize("all")]
        [HttpGet("{action}/{patientcaseid}")]
        public ActionResult<DeviceAssignmentBO> GetDeviceAssignmentByCase(long patientcaseid)
        {
            try
            {
                var obj = _patientService.GetDeviceAssignmentByCaseId(patientcaseid);
                if (obj == null)
                    return NotFound();
                return Ok(obj);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize("all")]
        [HttpPost("{action}")]
        public ActionResult AssignDevice(DeviceAssignmentBO deviceAssignment)
        {
            try
            {
                _patientService.AssignDevice(deviceAssignment);
                return Ok();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion
        #region Additional info
        [Authorize("all")]
        [HttpGet("cases/additionalinfo/{patientcaseid}")]
        public ActionResult<PatientAdditionalInfoBO> GetAdditionalInfo(long patientcaseid)
        {
            var pat = _additionalInfoService.GetPatientAdditionalInfoByCaseID(patientcaseid);
            if (pat == null)
            {
                return NotFound();
            }

            return Ok(pat);
        }

        [Authorize("all")]
        [HttpPost("cases/additionalinfo")]
        public ActionResult<PatientAdditionalInfoBO> AddPatientAdditionalInfo(PatientAdditionalInfoBO patientAdditionalInfo)
        {
            try
            {
                var id = _additionalInfoService.AddPatientAdditionalInfo(patientAdditionalInfo);
                return _additionalInfoService.GetPatientAdditionalInfoByCaseID(patientAdditionalInfo.PatientCaseID);
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return UnprocessableEntity(ex.Message);
            }
        }

        [Authorize("all")]
        [HttpPut("cases/additionalinfo")]
        public ActionResult<PatientAdditionalInfoBO> UpdatePatientAdditionalInfo(PatientAdditionalInfoBO patientAdditionalInfo)
        {
            try
            {
                var ph = _additionalInfoService.GetPatientAdditionalInfoByCaseID(patientAdditionalInfo.PatientCaseID);
                if (ph == null)
                {
                    return NotFound();
                }

                _additionalInfoService.UpdatePatientAdditionalInfo(patientAdditionalInfo);
                return _additionalInfoService.GetPatientAdditionalInfoByCaseID(patientAdditionalInfo.PatientCaseID);
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return UnprocessableEntity(ex.Message);
            }
        }
        #endregion
        #region Quick Evaluation Result
        [Authorize("all")]
        [HttpGet("cases/quickevaluationresult/{patientcaseid}")]
        public ActionResult<QuickEvaluationResultBO> GetQuickEvaluationResult(long patientcaseid)
        {
            var pat = _evaluationService.GetQuickEvaluationResultByCaseID(patientcaseid);
            if (pat == null)
            {
                return NotFound();
            }

            return Ok(pat);
        }

        [Authorize("all")]
        [HttpPost("cases/quickevaluationresult")]
        public ActionResult<QuickEvaluationResultBO> AddQuickEvaluationResult(QuickEvaluationResultBO quickEvaluationResult)
        {
            try
            {
                var id = _evaluationService.AddQuickEvaluationResult(quickEvaluationResult);
                return _evaluationService.GetQuickEvaluationResultByCaseID(quickEvaluationResult.PatientCaseID);
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return UnprocessableEntity(ex.Message);
            }
        }

        [Authorize("all")]
        [HttpPut("cases/quickevaluationresult")]
        public ActionResult<QuickEvaluationResultBO> UpdateQuickEvaluationResult(QuickEvaluationResultBO quickEvaluationResult)
        {
            try
            {
                var ph = _evaluationService.GetQuickEvaluationResultByCaseID(quickEvaluationResult.PatientCaseID);
                if (ph == null)
                {
                    return NotFound();
                }

                _evaluationService.UpdateQuickEvaluationResult(quickEvaluationResult);
                return _evaluationService.GetQuickEvaluationResultByCaseID(quickEvaluationResult.PatientCaseID);
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return UnprocessableEntity(ex.Message);
            }
        }
        #endregion
        [Authorize("all")]
        [HttpPost("cases/casedispatchdetails")]
        public ActionResult<CaseDispatchDetailBO> AddCaseDispatchDetails(CaseDispatchDetailBO caseDispatchDetailBO)
        {
            try
            {
                var id = _caseDispatchService.AddCaseDispatchDetails(caseDispatchDetailBO);
                return _caseDispatchService.GetCaseDispatchDetails(caseDispatchDetailBO.PatientCaseID);
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return UnprocessableEntity(ex.Message);
            }
        }
        [Authorize("all")]
        [HttpGet("cases/casedispatchdetails")]
        public ActionResult<CaseDispatchDetailBO> GetCaseDispatchDetails(long patientCaseID)
        {
            try
            {
                var cd = _caseDispatchService.GetCaseDispatchDetails(patientCaseID);
                if (cd == null)
                    return NotFound();
                return Ok(cd);
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return UnprocessableEntity(ex.Message);
            }
        }
        #region Drugs

        [Authorize("all")]
        [HttpGet("druggroups/patientcase/{patientcaseid}")]
        public ActionResult<List<DrugGroupBO>> GetDrugGroups(long patientcaseid)
        {
            var pat = _drugService.GetDrugGroups(patientcaseid);
            if (pat == null)
            {
                return NotFound();
            }

            return Ok(pat);
        }

        [Authorize("all")]
        [HttpGet("druggroups/{druggroupid}")]
        public ActionResult<DrugGroupBO> GetDrugGroup(long druggroupid)
        {
            var pat = _drugService.GetDrugGroup(druggroupid);
            if (pat == null)
            {
                return NotFound();
            }

            return Ok(pat);
        }

        [Authorize("all")]
        [HttpPost("druggroups")]
        public ActionResult<DrugGroupBO> AddDrugGroups(DrugGroupBO drugGroupBO)
        {
            try
            {
                var id = _drugService.AddDrugGroup(drugGroupBO);
                return _drugService.GetDrugGroup(id);
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return UnprocessableEntity(ex.Message);
            }
        }
        [Authorize("all")]
        [HttpPut("druggroups")]
        public ActionResult<DrugGroupBO> UpdateDrugGroups(DrugGroupBO drugGroupBO)
        {
            try
            {
                var ph = _drugService.GetDrugGroup(drugGroupBO.ID);
                if (ph == null)
                {
                    return NotFound();
                }
                _drugService.UpdateDrugGroup(drugGroupBO);
                return _drugService.GetDrugGroup(drugGroupBO.ID);
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return UnprocessableEntity(ex.Message);
            }
        }
        [Authorize("all")]
        [HttpDelete("druggroups")]
        public ActionResult DeleteDrugGroup(DrugGroupBO drugGroupBO)
        {
            try
            {
                var ph = _drugService.GetDrugGroupExist(drugGroupBO.ID);
                if (ph == null)
                {
                    return NotFound();
                }
                _drugService.DeleteDrugGroup(drugGroupBO);
                return Ok();
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return UnprocessableEntity(ex.Message);
            }
        }
        [Authorize("all")]
        [HttpPost("druggroups/details")]
        public ActionResult<DrugDetailsBO> AddDrugDetails(DrugDetailsBO drugDetailsBO)
        {
            try
            {
                var id = _drugService.AddDrugDetails(drugDetailsBO);
                return _drugService.GetDrugDetails(id);
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return UnprocessableEntity(ex.Message);
            }
        }
        [Authorize("all")]
        [HttpPut("druggroups/details")]
        public ActionResult<DrugDetailsBO> UpdateDrugDetails(DrugDetailsBO drugDetailsBO)
        {
            try
            {
                var ph = _drugService.GetDrugDetails(drugDetailsBO.ID);
                if (ph == null)
                {
                    return NotFound();
                }
                _drugService.UpdateDrugDetails(drugDetailsBO);
                return _drugService.GetDrugDetails(drugDetailsBO.ID);
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return UnprocessableEntity(ex.Message);
            }
        }
        [Authorize("all")]
        [HttpDelete("druggroups/details")]
        public ActionResult DeleteDrugDetails(DrugDetailsBO drugDetailsBO)
        {
            try
            {
                var ph = _drugService.GetDrugDetails(drugDetailsBO.ID);
                if (ph == null)
                {
                    return NotFound();
                }
                _drugService.DeleteDrugDetails(drugDetailsBO);
                return Ok();
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return UnprocessableEntity(ex.Message);
            }
        }
        [Authorize("all")]
        [HttpPost("druggroups/ingredients")]
        public ActionResult<DrugIngredientsBO> AddDrugIngredients(DrugIngredientsBO drugIngredientsBO)
        {
            try
            {
                var id = _drugService.AddDrugIngredients(drugIngredientsBO);
                return _drugService.GetDrugIngredients(id);
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return UnprocessableEntity(ex.Message);
            }
        }

        [Authorize("all")]
        [HttpPost("druggroups/freetext")]
        public ActionResult<DrugFreeTextBO> AddDrugFreeText(DrugFreeTextBO drugFreeTextBO)
        {
            try
            {
                var id = _drugService.AddDrugFreeText(drugFreeTextBO);
                return _drugService.GetDrugFreeText(id);
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return UnprocessableEntity(ex.Message);
            }
        }
        [Authorize("all")]
        [HttpPut("druggroups/freetext")]
        public ActionResult<DrugFreeTextBO> UpdateDrugFreeText(DrugFreeTextBO drugFreeTextBO)
        {
            try
            {
                var ph = _drugService.GetDrugFreeText(drugFreeTextBO.ID);
                if (ph == null)
                {
                    return NotFound();
                }
                _drugService.UpdateDrugFreeText(drugFreeTextBO);
                return _drugService.GetDrugFreeText(drugFreeTextBO.ID);
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return UnprocessableEntity(ex.Message);
            }
        }
        [Authorize("all")]
        [HttpDelete("druggroups/freetext")]
        public ActionResult DeleteDrugReceipe(DrugFreeTextBO drugFreeTextBO)
        {
            try
            {
                var ph = _drugService.GetDrugFreeText(drugFreeTextBO.ID);
                if (ph == null)
                {
                    return NotFound();
                }
                _drugService.DeleteDrugFreeText(drugFreeTextBO);
                return Ok();
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return UnprocessableEntity(ex.Message);
            }
        }
        [Authorize("all")]
        [HttpPost("druggroups/recipe")]
        public ActionResult<DrugReceipeBO> AddDrugReceipe(DrugReceipeBO drugReceipeBO)
        {
            try
            {
                var id = _drugService.AddDrugReceipe(drugReceipeBO);
                return _drugService.GetDrugReceipe(id);
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return UnprocessableEntity(ex.Message);
            }
        }
        [Authorize("all")]
        [HttpPut("druggroups/recipe")]
        public ActionResult<DrugReceipeBO> UpdateDrugReceipe(DrugReceipeBO drugReceipeBO)
        {
            try
            {
                var ph = _drugService.GetDrugReceipe(drugReceipeBO.ID);
                if (ph == null)
                {
                    return NotFound();
                }
                _drugService.UpdateDrugReceipe(drugReceipeBO);
                return _drugService.GetDrugReceipe(drugReceipeBO.ID);
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return UnprocessableEntity(ex.Message);
            }
        }
        [Authorize("all")]
        [HttpDelete("druggroups/recipe")]
        public ActionResult DeleteDrugReceipe(DrugReceipeBO drugReceipeBO)
        {
            try
            {
                var ph = _drugService.GetDrugReceipe(drugReceipeBO.ID);
                if (ph == null)
                {
                    return NotFound();
                }
                _drugService.DeleteDrugReceipe(drugReceipeBO);
                return Ok();
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return UnprocessableEntity(ex.Message);
            }
        }
        [Authorize("all")]
        [HttpGet("{action}")]
        public ActionResult<MedicationPlanFileBO> GetMedicationFile(long patientcaseid)
        {
            try
            {
                MedicationPlanFileBO edf = _drugService.GetMedicationFile(patientcaseid);
                if (edf == null)
                    return NotFound();
                return Ok(edf);
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return UnprocessableEntity(ex.Message);
            }
        }

        [Authorize("all")]
        [HttpGet("{action}")]
        public ActionResult DownloadMedicationFile(long patientcaseid)
        {
            MedicationPlanFileBO filedata;
            try
            {
                filedata = _drugService.DownloadMedicationFile(patientcaseid);
                if (filedata != null && filedata.FileData != null)
                    return File(filedata.FileData, filedata.ContentType, filedata.FileName);
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return UnprocessableEntity(ex.Message);
            }

            return NotFound();
        }
        [Authorize("all")]
        [HttpPost("{action}"), DisableRequestSizeLimit]
        public ActionResult UploadMedicationFile(long patientcaseid)
        {
            _logger.LogTrace($"UploadMedicationFile Enter {patientcaseid}");
            try
            {
                var file = Request.Form.Files[0];
                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fullPath = $"MP/{patientcaseid}/{System.Guid.NewGuid().ToString()}";
                    using (var stream = new MemoryStream())
                    {
                        file.CopyTo(stream);
                        _drugService.UploadMedicationFile(
                            new MedicationPlanFileBO
                            {
                                PatientCaseID = patientcaseid,
                                FilePath = fullPath,
                                FileName = fileName,
                                FileLength = file.Length,
                                ContentType = file.ContentType,
                                FileData = stream.GetBuffer()
                            }
                            );
                    }

                    return Ok();
                }
            }
            catch (ServiceException ex)
            {
                _logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (System.Exception ex)
            {
                return UnprocessableEntity(ex.Message);
            }

            return BadRequest(ClientSideErrors.FAILED_TO_UPLOAD);
        }

        #endregion
        #region Notes
        [Authorize("all")]
        [HttpGet("casenotes/{patientcaseid}")]
        public ActionResult<IEnumerable<CaseNotesBO>> GetCaseNotes(long patientcaseid)
        {
            var pat = _caseNotesService.GetCaseNotes(patientcaseid);
            if (pat == null)
            {
                return NotFound();
            }
            return Ok(pat);
        }
        [Authorize("all")]
        [HttpPost("casenotes")]
        public ActionResult<CaseNotesBO> AddCaseNotes(CaseNotesBO caseNotesBO)
        {
            try
            {
                var id = _caseNotesService.AddCaseNotes(caseNotesBO);
                return _caseNotesService.GetCaseNote(id);
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return UnprocessableEntity(ex.Message);
            }
        }
        #endregion
        #region Dashboard functions
        [Authorize("dashboard")]
        [HttpGet("dashboard/started/{pharmacyid}")]
        public ActionResult<long> GetNewCasesCount(long pharmacyid)
        {
            return Ok(_dashboardService.GetNewCasesCount(pharmacyid));
        }
        [Authorize("dashboard")]
        [HttpGet("dashboard/pending/{pharmacyid}")]
        public ActionResult<long> GetPendingCasesCount(long pharmacyid)
        {
            return Ok(_dashboardService.GetPendingCasesCount(pharmacyid));
        }
        [Authorize("dashboard")]
        [HttpGet("dashboard/qecompleted/{pharmacyid}")]
        public ActionResult<long> GetQECompletedCount(long pharmacyid)
        {
            return Ok(_dashboardService.GetQECompletedCount(pharmacyid));
        }
        [Authorize("dashboard")]
        [HttpGet("dashboard/detailcompleted/{pharmacyid}")]
        public ActionResult<long> GetDetailedCompletedCount(long pharmacyid)
        {
            return Ok(_dashboardService.GetDetailedCompletedCount(pharmacyid));
        }
        [Authorize("dashboard")]
        [HttpGet("dashboard/goalcompletedpercent")]
        public ActionResult<GoalCompletedBO> GetGoalCompletedPercent()
        {
            return Ok(_dashboardService.GetGoalCompletedPercent());
        }
        [Authorize("dashboard")]
        [HttpGet("dashboard/monthlycasesstarted")]
        public ActionResult<List<MonthlyCasesBO>> GetMonthlyCasesStarted()
        {
            return Ok(_dashboardService.GetMonthlyCasesStarted());
        }
        [Authorize("dashboard")]
        [HttpGet("dashboard/monthlycasescompleted")]
        public ActionResult<List<MonthlyCasesBO>> GetMonthlyCasesCompleted()
        {
            return Ok(_dashboardService.GetMonthlyCasesCompleted());
        }
        [Authorize("dashboard")]
        [HttpGet("dashboard/monthlycasesdispatched")]
        public ActionResult<List<MonthlyCasesBO>> GetMonthlyCasesDispatched()
        {
            return Ok(_dashboardService.GetMonthlyCasesDispatched());
        }
        [Authorize("dashboard")]
        [HttpGet("dashboard/cardiologistnotesstats")]
        public ActionResult<List<CaseStatsBO>> GetCardiologistNotesStats()
        {
            return Ok(_dashboardService.GetCardiologistNotesStats());
        }
        [Authorize("dashboard")]
        [HttpGet("dashboard/totalcardiologistassignedcases")]
        public ActionResult<long> GetTotalCardiologistAssignedCases()
        {
            return Ok(_dashboardService.GetTotalCardiologistAssignedCases());
        }
        [Authorize("dashboard")]
        [HttpGet("dashboard/totalcardiologistactivecases")]
        public ActionResult<long> GetTotalCardiologistActiveCases()
        {
            return Ok(_dashboardService.GetTotalCardiologistActiveCases());
        }
        [Authorize("dashboard")]
        [HttpGet("dashboard/qeresultstats")]
        public ActionResult<List<QEResultStatBO>> GetQEResultStats()
        {
            return Ok(_dashboardService.GetQEResultStats());
        }
        #endregion
    }
}