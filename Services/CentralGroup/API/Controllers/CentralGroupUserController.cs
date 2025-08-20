using CentralGroup.Contracts.Interfaces;
using CentralGroup.Contracts.Models;
using Common.BusinessObjects;
using Common.BusinessObjects.Interfaces;
using Common.Infrastructure;
using Common.Infrastructure.Exceptions;
using Common.Services.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Patient.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Contracts.Models;

namespace CentralGroup.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class CentralGroupUserController : ControllerBase
    {
        private readonly ILogger<CentralGroupUserController> _logger;
        private ICentralGroupService _centralGroupService;
        private readonly ICommonUserServce _commonUserService;
        private readonly IPatientService _patientService;
        private readonly IDECompletedEventNotificationService _centralGroupNotificationService;
        public CentralGroupUserController(ILogger<CentralGroupUserController> logger, ICentralGroupService centralGroupService, ICommonUserServce commonUserService,
            IDECompletedEventNotificationService centralGroupNotificationService, IPatientService patientService)
        {
            _logger = logger;
            _centralGroupService = centralGroupService;
            _commonUserService = commonUserService;
            _patientService = patientService;
            _centralGroupNotificationService = centralGroupNotificationService;
        }

        [HttpGet]
        public ActionResult<PagedResults<CentralGroupBO>> Get(int offset = 0, int limit = 0, string orderby = null, string filter = null)
        {
            var ph = _centralGroupService.GetCentralGroup(limit, offset, orderby, filter);
            if (ph == null)
            {
                return NotFound();
            }
            return Ok(ph);
        }
        [HttpGet("{id}")]
        public ActionResult<CentralGroupBO> Get(int id)
        {
            var ph = _centralGroupService.GetCentralGroupById(id);
            if (ph == null)
            {
                return NotFound();
            }
            return ph;
        }
        [HttpGet("total")]
        public ActionResult<long> GetTotal()
        {
            return Ok(_centralGroupService.GetTotal());
        }
        [HttpGet("email/{loginid}")]
        public ActionResult<CentralGroupBO> Get(string loginid)
        {
            var ph = _centralGroupService.GetCentralGroupByEmail(loginid);
            if (ph == null)
            {
                return NotFound();
            }
            return ph;
        }
        [HttpPost]
        public ActionResult<CentralGroupBO> AddCentralGroup(CentralGroupBO centralGroup)
        {
            try
            {
                SessionContext sessionContext = Request.HttpContext.GetSessionContext();
                centralGroup.CreatedBy = sessionContext.LoginName;
                var id = _centralGroupService.AddCentralGroup(centralGroup);
                return _centralGroupService.GetCentralGroupById(id);
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
        public ActionResult<CentralGroupBO> UpdateCentralGroup(CentralGroupBO centralGroup)
        {
            try
            {
                SessionContext sessionContext = Request.HttpContext.GetSessionContext();

                var ph = _centralGroupService.GetCentralGroupById(centralGroup.ID);
                if (ph == null)
                {
                    return NotFound();
                }
                ph.UpdatedBy = sessionContext.LoginName;
                _centralGroupService.UpdateCentralGroup(centralGroup);
                return _centralGroupService.GetCentralGroupById(centralGroup.ID);
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
        public ActionResult<CentralGroupBO> DeleteCentralGroup(long id)
        {
            try
            {
                SessionContext sessionContext = Request.HttpContext.GetSessionContext();
                var ph = _centralGroupService.GetCentralGroupById(id);
                if (ph == null)
                {
                    return NotFound();
                }
                ph.UpdatedBy = sessionContext.LoginName;
                _centralGroupService.DeleteCentralGroup(ph);
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
        [HttpGet("settings")]
        public ActionResult<List<UserSettingBO>> GetSettings()
        {
            SessionContext sessionContext = Request.HttpContext.GetSessionContext();
            var ph = _commonUserService.GetSettings(sessionContext.LoginName);
            if (ph == null)
            {
                return NotFound();
            }
            return ph;
        }
        [HttpPut("updatesettings")]
        public ActionResult UpdateSettings(List<UserSettingBO> userSettings)
        {
            try
            {
                SessionContext sessionContext = Request.HttpContext.GetSessionContext();
                if (_centralGroupService.GetCentralGroupByEmail(sessionContext.LoginName).PhoneVerified == false
                    && userSettings.FirstOrDefault(p => p.Code == UserSettingCodes.FACTOR_NOTIFICATION_TYPE)?.Value == NotificationType.SMS.ToString())
                    throw new ServiceException(ClientSideErrors.PHONE_IS_UNVERIFIED);
                _commonUserService.UpdateSettings(sessionContext.LoginName, userSettings);
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
        [HttpGet("eventemailtemplate/eventtypeid")]
        public ActionResult<NotificationTemplateBO> Get(long eventtypeid)
        {
            try
            {
                var v = _centralGroupNotificationService.GetEmailNotificationTemplate(eventtypeid);
                return Ok(v);
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
        #region patient service functions
        [HttpPost("cases/casedispatchdetails")]
        public ActionResult<CaseDispatchDetailBO> AddCaseDispatchDetails(CaseDispatchDetailBO caseDispatchDetailBO)
        {
            try
            {
                return _patientService.AddCaseDispatchDetails(caseDispatchDetailBO);
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
        [HttpPost("cases/casedispatchdetails")]
        public ActionResult<CaseDispatchDetailBO> GetCaseDispatchDetails(long patientcaseid)
        {
            try
            {
                var v = _patientService.GetCaseDispatchDetails(patientcaseid);
                if (v == null)
                    return NotFound();
                return Ok(v);
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
        #region Phone Verification
        [HttpPost("generatephoneverification")]
        public ActionResult<object> GetToken(VerifyPhoneRequest req)
        {
            try
            {
                var v = _centralGroupService.GeneratePhoneVerification(req);
                return Ok(new { token = v });
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
        [HttpPost("phoneverification")]
        public ActionResult<CentralGroupBO> VerifyToken(VerifyPhoneOtpRequest req)
        {
            try
            {
                var v = _centralGroupService.VerifyPhoneVerification(req);
                return Ok(v);
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

        #region Change Password

        [HttpPost("changepassword")]
        public async Task<ActionResult<object>> ChangePassword(ChangeCredentialBO req)
        {
            try
            {
                SessionContext sessionContext = Request.HttpContext.GetSessionContext();
                req.UserId = sessionContext.UserID;
                var ph = _centralGroupService.GetCentralGroupById(sessionContext.AppUserID);
                if (ph == null)
                {
                    return NotFound();
                }

                await _commonUserService.ChangeCredentials(sessionContext.LoginName, req);

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
    }
}
