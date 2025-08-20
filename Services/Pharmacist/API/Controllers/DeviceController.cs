using Common.Infrastructure;
using Common.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pharmacist.Contracts.Interfaces;
using Pharmacist.Contracts.Models;
using System;

namespace Pharmacist.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class DeviceController : ControllerBase
    {
        private readonly ILogger<DeviceController> _logger;
        private IDeviceService _deviceService;

        public DeviceController(ILogger<DeviceController> logger, IDeviceService pharma)
        {
            _logger = logger;
            _deviceService = pharma;
        }
        [HttpGet]
        public ActionResult<PagedResults<DeviceBO>> Get(int offset = 0, int limit = 0, string orderby = null, string filter = null)
        {
            var ph = _deviceService.GetDevices(limit, offset, orderby, filter);
            if (ph == null)
            {
                return NotFound();
            }
            return Ok(ph);
        }
        [HttpGet("pharmacy")]
        public ActionResult<PagedResults<DeviceBO>> GetDevicesByPharmacy(int offset = 0, int limit = 0, string orderby = null, string filter = null)
        {
            var ph = _deviceService.GetDevicesByPharmacy(limit, offset, orderby, filter);
            if (ph == null)
            {
                return NotFound();
            }
            return Ok(ph);
        }
        [HttpGet("{id}")]
        public ActionResult<DeviceBO> Get(int id)
        {
            var ph = _deviceService.GetDeviceById(id);
            if (ph == null)
            {
                return NotFound();
            }
            return ph;
        }
        [HttpPost]
        public ActionResult<DeviceBO> AddDevice(DeviceBO device)
        {
            try
            {
                if (_deviceService.GetDeviceBySerial(device.SerialNumber) != null)
                    return BadRequest(ClientSideErrors.DEVICE_SERIAL_ALREADY_EXIST);
                var id = _deviceService.AddDevice(device);
                return _deviceService.GetDeviceById(id);
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
        public ActionResult<DeviceBO> UpdateDevice(DeviceBO device)
        {
            try
            {
                var old = _deviceService.GetDeviceBySerial(device.SerialNumber);
                if (old != null && old.ID != device.ID)
                    return BadRequest(ClientSideErrors.DEVICE_SERIAL_ALREADY_EXIST);
                var ph = _deviceService.GetDeviceById(device.ID);
                if (ph == null)
                {
                    return NotFound();
                }
                _deviceService.UpdateDevice(device);
                return _deviceService.GetDeviceById(device.ID);
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
        public ActionResult<DeviceBO> DeleteDevice(long id)
        {
            try
            {
                var ph = _deviceService.GetDeviceById(id);
                if (ph == null)
                {
                    return NotFound();
                }
                _deviceService.DeleteDevice(ph);
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

        [HttpPost("{action}")]
        public ActionResult<DeviceBO> AssignDevice([FromBody] DeviceAssignmentBO deviceAssignment)
        {
            try
            {
                return Ok(_deviceService.AssignDevice(deviceAssignment));
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

        [HttpGet("{action}/{patientcaseid}")]
        public ActionResult<DeviceAssignmentBO> GetDeviceAssignment(long patientcaseid)
        {
            try
            {
                var obj = _deviceService.GetDeviceAssignmentByPatientCaseID(patientcaseid);
                if (obj == null)
                    return NotFound();
                return Ok(obj);
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
