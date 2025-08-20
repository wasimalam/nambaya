using Common.BusinessObjects;
using Common.Infrastructure;
using Common.Infrastructure.Exceptions;
using Common.Services.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Patient.Contracts.Interfaces;
using Patient.Contracts.Models;
using System;

namespace Patient.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class DoctorController : ControllerBase
    {
        private readonly ILogger<DoctorController> _logger;
        private IDoctorService _doctorService;
        public DoctorController(ILogger<DoctorController> logger, IDoctorService service)
        {
            _logger = logger;
            _doctorService = service;
        }
        [HttpGet]
        public ActionResult<PagedResults<DoctorBO>> Get(int offset = 0, int limit = 0, string orderby = null, string filter = null)
        {
            var ph = _doctorService.GetDoctors(limit, offset, orderby, filter);
            if (ph == null)
            {
                return NotFound();
            }
            return Ok(ph);
        }
        [HttpGet("{id}")]
        public ActionResult<DoctorBO> Get(long id)
        {
            var ph = _doctorService.GetDoctorById(id);
            if (ph == null)
            {
                return NotFound();
            }
            return ph;
        }

        [HttpPost]
        public ActionResult<DoctorBO> AddDoctor(DoctorBO doctor)
        {
            try
            {
                var id = _doctorService.AddDoctor(doctor);
                return _doctorService.GetDoctorById(id);
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
        public ActionResult<DoctorBO> UpdateDoctor(DoctorBO doctor)
        {
            try
            {
                var ph = _doctorService.GetDoctorById(doctor.ID);
                if (ph == null)
                {
                    return NotFound();
                }
                _doctorService.UpdateDoctor(doctor);
                return _doctorService.GetDoctorById(doctor.ID);
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
        [HttpDelete("{id}")]
        public ActionResult<DoctorBO> DeleteDoctor(long id)
        {
            try
            {
                SessionContext sessionContext = Request.HttpContext.GetSessionContext();
                var ph = _doctorService.GetDoctorById(id);
                if (ph == null)
                {
                    return NotFound();
                }
                _doctorService.DeleteDoctor(ph);
                return ph;
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
    }
}
