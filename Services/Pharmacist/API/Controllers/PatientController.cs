using Common.Infrastructure;
using Common.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Patient.Contracts.Models;
using Pharmacist.Contracts.Interfaces;
using Pharmacist.Contracts.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;

namespace Pharmacist.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class PatientController : ControllerBase
    {
        private readonly ILogger<PatientController> _logger;
        private IPatientService _patientService;

        public PatientController(ILogger<PatientController> logger, IPatientService pharma)
        {
            _logger = logger;
            _patientService = pharma;
        }
        [HttpGet]
        public ActionResult<PagedResults<PatientBO>> GetCases(long pharmacyid, int offset = 0, int limit = 0, string orderby = null, string filter = null)
        {
            var pat = _patientService.GetPatientCases(limit, offset, orderby, filter);
            if (pat == null)
            {
                return NotFound();
            }

            return Ok(pat);
        }

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

        [HttpPost]
        public ActionResult<PatientBO> AddPatient(PatientBO patient)
        {
            try
            {
                return _patientService.AddPatient(patient);
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

        [HttpPut]
        public ActionResult<PatientBO> UpdatePatient(PatientBO patient)
        {
            try
            {
                return _patientService.UpdatePatient(patient);
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
        [HttpGet("patientdeactivateverification")]
        public ActionResult GetDeActivatePatient(long patientId)
        {
            try
            {
                var token =  _patientService.GenerateDeActivatePatientVerification(patientId);
                return Ok(new { token = token, ttl = 300 });
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
        [HttpPost("patientdeactivateverification")]
        public ActionResult VerifyToken(VerifyDeactivatePatientOtpRequest req)
        {
            try
            {
                _patientService.VerifyDeActivatePatient(req);
                return Ok();
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
                return BadRequest(ClientSideErrors.INVALID_PATIENT_CASE_STEP);
            _patientService.UpdatePatientCase(pat);
            return Ok(_patientService.GetPatientbyCaseID(patientcaseid));
        }
        [HttpPost("{action}"), DisableRequestSizeLimit]
        public ActionResult<PatientBO> ImportPatientFile(string pharmacypatientid)
        {
            _logger.LogTrace($"ImportPatientFile Enter {pharmacypatientid}");
            try
            {
                var file = Request.Form.Files[0];
                if (file.Length > 0)
                {
                    using (var stream = new MemoryStream())
                    {
                        var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        file.CopyTo(stream);
                        var bytes = stream.GetBuffer();
                        return Ok(_patientService.ImportXml(bytes, pharmacypatientid, fileName, file.ContentType));
                    }
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
            return BadRequest("FAILED_TO_UPLOAD");
        }
        [HttpGet("{action}")]
        public ActionResult<PatientEDFFileBO> GetPatientEDFFile(long patientcaseid)
        {
            try
            {
                var edf = _patientService.GetPatientEDFFile(patientcaseid);
                if (edf == null)
                    return NotFound();
                return Ok(edf);
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
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return UnprocessableEntity(ex.Message);
            }
            return NotFound();
        }
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
                        _patientService.UploadEDFFile(
                            new PatientEDFFileBO
                            {
                                PatientCaseID = patientcaseid,
                                FilePath = fullPath,
                                FileName = fileName,
                                FileLength = file.Length,
                                ContentType = file.ContentType
                            },
                            stream.GetBuffer()
                            );
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

            return BadRequest("FAILED_TO_UPLOAD");
        }
        [HttpGet("{action}")]
        public ActionResult DownloadQuickEvaluationFile(long patientcaseid)
        {
            QuickEvaluationFileBO filedata;
            try
            {
                filedata = _patientService.DownloadQuickEvalFile(patientcaseid);
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
        [HttpGet("{action}")]
        public ActionResult DownloadQuickEvaluationImages(long patientcaseid)
        {
            try
            {
                List<string> imageBytes;
                imageBytes = _patientService.DownloadQuickEvalImages(patientcaseid);
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
        [HttpGet("{action}")]
        public ActionResult GetNavigatorStatus(long patientcaseid)
        {
            try
            {
                List<string> imageBytes;
                imageBytes = _patientService.DownloadQuickEvalImages(patientcaseid);
                return Ok(imageBytes);
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
        [HttpGet("{action}")]
        public ActionResult UpdateNavigatorStatus(long patientcaseid)
        {
            try
            {
                List<string> imageBytes;
                imageBytes = _patientService.DownloadQuickEvalImages(patientcaseid);
                return Ok(imageBytes);
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
        [HttpPost("{action}"), DisableRequestSizeLimit, RequestFormLimits(MultipartBodyLengthLimit = System.Int32.MaxValue, ValueLengthLimit = System.Int32.MaxValue)]
        public ActionResult UploadQuickEvalFile(long patientcaseid)
        {
            _logger.LogTrace($"UploadQuickEvalFile Enter {patientcaseid}");
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
                        _patientService.UploadQuickEvalFile(
                            new QuickEvaluationFileBO()
                            {
                                PatientCaseID = patientcaseid,
                                FilePath = fullPath,
                                FileName = fileName,
                                FileLength = file.Length,
                                ContentType = file.ContentType
                            },
                            stream.GetBuffer()
                        );
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

            return BadRequest("FAILED_TO_UPLOAD");
        }

        #region QuickE valuation Result
        [HttpGet("cases/quickevaluationresult/{patientcaseid}")]
        public ActionResult<QuickEvaluationResultBO> GetQuickEvaluationResult(long patientcaseid)
        {
            var pat = _patientService.GetQuickEvaluationResultByCaseID(patientcaseid);
            if (pat == null)
            {
                return NotFound();
            }
            return Ok(pat);
        }

        [HttpPost("cases/quickevaluationresult")]
        public ActionResult<QuickEvaluationResultBO> AddQuickEvaluationResult(QuickEvaluationResultBO quickEvaluationResult)
        {
            try
            {
                return Ok(_patientService.AddQuickEvaluationResult(quickEvaluationResult));
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

        [HttpPut("cases/quickevaluationresult")]
        public ActionResult<QuickEvaluationResultBO> UpdateQuickEvaluationResult(QuickEvaluationResultBO quickEvaluationResult)
        {
            try
            {
                return Ok(_patientService.UpdateQuickEvaluationResult(quickEvaluationResult));
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

        [HttpPost("cases/additionalinfo")]
        public ActionResult<PatientAdditionalInfoBO> AddPatientAdditionalInfo(PatientAdditionalInfoBO patientAdditionalInfo)
        {
            try
            {
                return Ok(_patientService.AddPatientAdditionalInfo(patientAdditionalInfo));
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

        [HttpPut("cases/additionalinfo")]
        public ActionResult<PatientAdditionalInfoBO> UpdatePatientAdditionalInfo(PatientAdditionalInfoBO patientAdditionalInfo)
        {
            try
            {
                return Ok(_patientService.UpdatePatientAdditionalInfo(patientAdditionalInfo));
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

        [HttpPost("druggroups")]
        public ActionResult<DrugGroupBO> AddDrugGroups(DrugGroupBO drugGroupBO)
        {
            try
            {
                return Ok(_patientService.AddDrugGroup(drugGroupBO));
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
        [HttpPut("druggroups")]
        public ActionResult<DrugGroupBO> UpdateDrugGroups(DrugGroupBO drugGroupBO)
        {
            try
            {
                var dg = _patientService.UpdateDrugGroup(drugGroupBO);
                if (dg == null)
                    return NotFound();
                return Ok(dg);
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
        [HttpDelete("druggroups")]
        public ActionResult DeleteDrugGroup(DrugGroupBO drugGroupBO)
        {
            try
            {
                _patientService.DeleteDrugGroup(drugGroupBO);
                return Ok();
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
        [HttpPost("druggroups/details")]
        public ActionResult<DrugDetailsBO> AddDrugDetails(DrugDetailsBO drugDetailsBO)
        {
            try
            {
                return Ok(_patientService.AddDrugDetails(drugDetailsBO));
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
        [HttpPut("druggroups/details")]
        public ActionResult<DrugDetailsBO> UpdateDrugDetails(DrugDetailsBO drugDetailsBO)
        {
            try
            {
                return Ok(_patientService.UpdateDrugDetails(drugDetailsBO));
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
        [HttpDelete("druggroups/details")]
        public ActionResult DeleteDrugDetails(DrugDetailsBO drugDetailsBO)
        {
            try
            {
                _patientService.DeleteDrugDetails(drugDetailsBO);
                return Ok();
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
        [HttpPost("druggroups/ingredients")]
        public ActionResult<DrugIngredientsBO> AddDrugIngredients(DrugIngredientsBO drugIngredientsBO)
        {
            try
            {
                return Ok(_patientService.AddDrugIngredients(drugIngredientsBO));
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

        [HttpPost("druggroups/freetext")]
        public ActionResult<DrugFreeTextBO> AddDrugFreeText(DrugFreeTextBO drugFreeTextBO)
        {
            try
            {
                return Ok(_patientService.AddDrugFreeText(drugFreeTextBO));
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
        [HttpPut("druggroups/freetext")]
        public ActionResult<DrugFreeTextBO> UpdateDrugFreeText(DrugFreeTextBO drugFreeTextBO)
        {
            try
            {
                return Ok(_patientService.UpdateDrugFreeText(drugFreeTextBO));
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
        [HttpDelete("druggroups/freetext")]
        public ActionResult DeleteDrugReceipe(DrugFreeTextBO drugFreeTextBO)
        {
            try
            {
                _patientService.DeleteDrugFreeText(drugFreeTextBO);
                return Ok();
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
        [HttpPost("druggroups/recipe")]
        public ActionResult<DrugReceipeBO> AddDrugReceipe(DrugReceipeBO drugReceipeBO)
        {
            try
            {
                return Ok(_patientService.AddDrugReceipe(drugReceipeBO));
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
        [HttpPut("druggroups/recipe")]
        public ActionResult<DrugReceipeBO> UpdateDrugReceipe(DrugReceipeBO drugReceipeBO)
        {
            try
            {
                return Ok(_patientService.UpdateDrugReceipe(drugReceipeBO));
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
        [HttpDelete("druggroups/recipe")]
        public ActionResult DeleteDrugReceipe(DrugReceipeBO drugReceipeBO)
        {
            try
            {
                _patientService.DeleteDrugReceipe(drugReceipeBO);
                return Ok();
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
        #region Dashboard
        [HttpGet("dashboard/pharmacystats")]
        public ActionResult<object> GetPharmacyStats()
        {
            try
            {
                return Ok(_patientService.GetPharmacyStats());
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
        [HttpGet("dashboard/monthlycasesstarted")]
        public ActionResult<List<MonthlyCasesBO>> GetMonthlyCasesStarted()
        {
            try
            {
                return Ok(_patientService.GetMonthlyCasesStarted());
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