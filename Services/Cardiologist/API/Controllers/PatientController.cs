using Cardiologist.Contracts.Interfaces;
using Cardiologist.Contracts.Models;
using Common.Infrastructure;
using Common.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Patient.Contracts.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Cardiologist.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class PatientController : ControllerBase
    {
        private readonly ILogger<PatientController> _logger;
        private readonly IPatientService _patientService;
        public PatientController(ILogger<PatientController> logger, IPatientService patientService)
        {
            _logger = logger;
            _patientService = patientService;
        }

        [HttpGet]
        public ActionResult<PagedResults<PatientBO>> Get(int limit = 0, int offset = 0, string orderby = null, string filter = null)
        {
            var pat = _patientService.GetPatients(limit, offset, orderby, filter);
            if (pat == null)
            {
                return NotFound();
            }

            return Ok(pat);
        }

        [HttpGet("cases")]
        public ActionResult<PagedResults<PatientBO>> GetCases(int offset = 0, int limit = 0, string orderby = null, string filter = null)
        {
            try
            {
                var pat = _patientService.GetPatientCases(limit, offset, orderby, filter);
                if (pat == null)
                {
                    return NotFound();
                }

                return Ok(pat);
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

        [HttpGet("{id}")]
        public ActionResult<PatientBO> Get(long id)
        {
            try
            {
                var pat = _patientService.GetPatientbyID(id);
                if (pat == null)
                {
                    return NotFound();
                }

                return Ok(pat);
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
        [HttpGet("cases/{patientcaseid}")]
        public ActionResult<PatientBO> GetPatientbyCaseID(long patientcaseid)
        {
            try
            {
                var pat = _patientService.GetPatientbyCaseID(patientcaseid);
                if (pat == null)
                {
                    return NotFound();
                }

                return Ok(pat);
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

        [HttpPut("cases/step/{patientcaseid}")]
        public ActionResult<PatientBO> UpdateCaseStep(long patientcaseid, long stepid)
        {
            var pat = _patientService.GetPatientCasebyID(patientcaseid);
            if (pat == null)
            {
                return NotFound();
            }
            pat.StepID = stepid;
            if (stepid < CaseStep.Created || stepid > CaseStep.QuickEvaluation)
            {
                _logger.LogWarning(ClientSideErrors.INVALID_PATIENT_CASE_STEP);
                return BadRequest(ClientSideErrors.INVALID_PATIENT_CASE_STEP);
            }
                
            _patientService.UpdatePatientCase(pat);
            return Ok(_patientService.GetPatientbyCaseID(patientcaseid));
        }

        [HttpGet("{action}")]
        public ActionResult DownloadEDFFile(long patientcaseid)
        {
            byte[] filedata;
            try
            {
                filedata = _patientService.DownloadEDFFile(patientcaseid);
                if (filedata != null)
                    return File(filedata, "application/octet-stream");
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
        [HttpGet("UploadEcgToken")]
        public ActionResult<object> GetUploadEcgToken(long patientcaseid)
        {
            try
            {
                var v = _patientService.GenerateDESignatureOTP(patientcaseid);
                return Ok(new { token = v, ttl = 300 });
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
        [HttpPost("{action}"), DisableRequestSizeLimit, RequestFormLimits(MultipartBodyLengthLimit = System.Int32.MaxValue, ValueLengthLimit = System.Int32.MaxValue)]
        public ActionResult UploadECGFileUnsigned()
        {
            try
            {
                var file = Request.Form.Files.FirstOrDefault();
                ECGUploadRequest req = JsonSerializer.Deserialize<ECGUploadRequest>(Request.Form["req"],
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                _logger.LogTrace($"UploadECGFileUnsigned Enter {req.PatientCaseId}");
                if (file != null && file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fullPath = $"ECG/{req.PatientCaseId}/{Guid.NewGuid()}";
                    using (var stream = new MemoryStream())
                    {
                        file.CopyTo(stream);
                        _patientService.UploadDetailEvaluationFile(
                            new DetailEvaluationBO
                            {
                                PatientCaseID = req.PatientCaseId,
                                ResultsPath = fullPath,
                                FileName = fileName,
                                FileLength = file.Length,
                                ContentType = file.ContentType,
                                NotesTypeID = (long?)(req.NotesTypeId == 0 ? ((long?)null) : req.NotesTypeId),
                                Notes = req.Notes,
                                IsSigned = false
                            },
                            stream.GetBuffer()
                       );
                    }
                }
                else
                {
                    _patientService.UpdateDetailEvaluationData(new DetailEvaluationBO
                    {
                        PatientCaseID = req.PatientCaseId,
                        NotesTypeID = (long?)(req.NotesTypeId == 0 ? ((long?)null) : req.NotesTypeId),
                        Notes = req.Notes,
                        IsSigned = false
                    });
                }
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
        [HttpPost("{action}"), DisableRequestSizeLimit, RequestFormLimits(MultipartBodyLengthLimit = System.Int32.MaxValue, ValueLengthLimit = System.Int32.MaxValue)]
        public ActionResult UploadECGFile()
        {
            try
            {
                var file = Request.Form.Files.FirstOrDefault();
                VerifyECGUploadOtpRequest req = JsonSerializer.Deserialize<VerifyECGUploadOtpRequest>(Request.Form["req"],
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                _logger.LogTrace($"UploadECGFile Enter {req.PatientCaseId}");
                if (file != null && file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');

                    var fullPath = $"ECG/{req.PatientCaseId}/{Guid.NewGuid()}";
                    using (var stream = new MemoryStream())
                    {
                        file.CopyTo(stream);
                        _patientService.VerifyECGFileUploadSignature(req,
                            new DetailEvaluationBO
                            {
                                PatientCaseID = req.PatientCaseId,
                                ResultsPath = fullPath,
                                FileName = fileName,
                                FileLength = file.Length,
                                ContentType = file.ContentType,
                                NotesTypeID = (long?)(req.NotesTypeId == 0 ? ((long?)null) : req.NotesTypeId),
                                Notes = req.Notes,
                                FileData = stream.GetBuffer()
                            }
                       );
                    }

                }
                else //if file data is not provided then old file will be signed
                {
                    _patientService.VerifyExistingECGFileWithSignature(req);
                }
                return Ok();
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
        #region Additional info
        [HttpGet("cases/additionalinfo/{patientcaseid}")]
        public ActionResult<PatientAdditionalInfoBO> GetAdditionalInfo(long patientcaseid)
        {
            var pat = _patientService.GetPatientAdditionalInfoByCaseID(patientcaseid);
            if (pat == null)
            {
                return NotFound();
            }

            return Ok(pat);
        }
        #endregion
        #region Drugs
        [HttpGet("druggroups/patientcase/{patientcaseid}")]
        public ActionResult<List<DrugGroupBO>> GetDrugGroups(long patientcaseid)
        {
            var pat = _patientService.GetDrugGroups(patientcaseid);
            if (pat == null)
            {
                return NotFound();
            }

            return Ok(pat);
        }

        [HttpGet("druggroups/{druggroupid}")]
        public ActionResult<DrugGroupBO> GetDrugGroup(long druggroupid)
        {
            var pat = _patientService.GetDrugGroup(druggroupid);
            if (pat == null)
            {
                return NotFound();
            }

            return Ok(pat);
        }
        #endregion
        #region Dashboard
        [HttpGet("dashboard/goalcompletedpercent")]
        public ActionResult<GoalCompletedBO> GetGoalcompletedPercent()
        {
            try
            {
                return Ok(_patientService.GetGoalcompletedPercent());
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
        [HttpGet("dashboard/monthlycasescompleted")]
        public ActionResult<List<MonthlyCasesBO>> GetMonthlyCasesCompleted()
        {
            try
            {
                return Ok(_patientService.GetMonthlyCasesCompleted());
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return UnprocessableEntity(ex.Message);
            }
        }
        [HttpGet("dashboard/totalcardiologistassignedcases")]
        public ActionResult<long> GetTotalCardiologistAssignedCases()
        {
            try
            {
                return Ok(_patientService.GetTotalCardiologistAssignedCases());
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
        [HttpGet("dashboard/totalcardiologistactivecases")]
        public ActionResult<long> GetTotalCardiologistActiveCases()
        {
            try
            {
                return Ok(_patientService.GetTotalCardiologistActiveCases());
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
        [HttpGet("dashboard/cardiologistnotesstats")]
        public ActionResult<List<MonthlyCasesBO>> GetCardiologistNotesStats()
        {
            try
            {
                return Ok(_patientService.GetCardiologistNotesStats());
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
        [HttpGet("dashboard/qeresultstats")]
        public ActionResult<List<QEResultStatBO>> GetQEResultStats()
        {
            try
            {
                return Ok(_patientService.GetQEResultStats());
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
    }
}