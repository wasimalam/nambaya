using Common.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NambayaUser.Contracts.Interfaces;
using Patient.Contracts.Models;
using System;
using System.Collections.Generic;

namespace NambayaUser.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class PatientController : ControllerBase
    {
        private readonly ILogger<PatientController> _logger;
        private readonly IPatientService _patientService;

        public PatientController(ILogger<PatientController> logger, IPatientService pharma)
        {
            _logger = logger;
            _patientService = pharma;
        }
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