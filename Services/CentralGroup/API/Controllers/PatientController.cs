using CentralGroup.Contracts.Interfaces;
using Common.Infrastructure;
using Common.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Patient.Contracts.Models;
using System;
using System.Collections.Generic;

namespace CentralGroup.API.Controllers
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



        [HttpGet("cases/pharmacy")]
        public ActionResult<PagedResults<PatientBO>> GetCases(int offset = 0, int limit = 0, string orderby = null, string filter = null)
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
        #region Evaluation
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
        [HttpGet("cases/evaluations/{patientcaseid}")]
        public ActionResult<DetailEvaluationBO> GetDetailEvaluation(long patientcaseid)
        {
            var pat = _patientService.GetDetailEvaluationByCaseID(patientcaseid);
            if (pat == null)
            {
                return NotFound();
            }
            return Ok(pat);
        }
        [HttpGet("{action}")]
        public ActionResult DownloadDetailEvaluationFile(long patientcaseid)
        {
            byte[] filedata;
            try
            {
                var pat = _patientService.GetDetailEvaluationByCaseID(patientcaseid);
                if (pat == null)
                {
                    return NotFound();
                }
                filedata = _patientService.DownloadDetailEvaluationFile(patientcaseid);
                if (filedata != null)
                    return File(filedata, pat.ContentType);
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
        [HttpGet("dashboard/monthlycasesdispatched")]
        public ActionResult<List<MonthlyCasesBO>> GetMonthlyCasesDispatched()
        {
            try
            {
                return Ok(_patientService.GetMonthlyCasesDispatched());
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