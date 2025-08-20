using Common.BusinessObjects;
using Common.BusinessObjects.Interfaces;
using Common.Infrastructure;
using Common.Infrastructure.Exceptions;
using Common.Services.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NambayaUser.Contracts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Contracts.Models;

namespace NambayaUser.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class NambayaUserController : ControllerBase
    {
        private readonly ILogger<NambayaUserController> _logger;
        private INambayaUserService _nambayaUserService;
        private readonly ICommonUserServce _commonUserService;
        private readonly INotificationSetupWrapperService _notificationSetupService;
        private readonly IConfiguration _configuration;
        public NambayaUserController(ILogger<NambayaUserController> logger, INambayaUserService nambayauser,
            ICommonUserServce commonUserService, INotificationSetupWrapperService notificationSetupService,
            IConfiguration configuration)
        {
            _logger = logger;
            _nambayaUserService = nambayauser;
            _notificationSetupService = notificationSetupService;
            _commonUserService = commonUserService;
            _configuration = configuration;
        }

        [HttpGet]
        public ActionResult<PagedResults<Contracts.Models.UserBO>> Get(int offset = 0, int limit = 0, string orderby = null, string filter = null)
        {
            var ph = _nambayaUserService.GetUsers(limit, offset, orderby, filter);
            if (ph == null)
            {
                return NotFound("NO_VALID_USER");
            }
            return Ok(ph);
        }
        [HttpGet("{id}")]
        public ActionResult<Contracts.Models.UserBO> Get(int id)
        {
            var ph = _nambayaUserService.GetUserById(id);
            if (ph == null)
            {
                return NotFound();
            }
            return ph;
        }
        [HttpGet("total")]
        public ActionResult<long> GetTotal()
        {
            return Ok(_nambayaUserService.GetTotal());
        }
        [HttpGet("email/{loginid}")]
        public ActionResult<Contracts.Models.UserBO> Get(string loginid)
        {
            var ph = _nambayaUserService.GetUserByEmail(loginid);
            if (ph == null)
            {
                return NotFound();
            }
            return ph;
        }
        [HttpPost]
        public ActionResult<Contracts.Models.UserBO> AddUser(Contracts.Models.UserBO user)
        {
            try
            {
                var id = _nambayaUserService.AddUser(user);
                return _nambayaUserService.GetUserById(id);
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
        public ActionResult<Contracts.Models.UserBO> UpdateUser(Contracts.Models.UserBO user)
        {
            try
            {
                var ph = _nambayaUserService.GetUserById(user.ID);
                if (ph == null)
                {
                    return NotFound();
                }
                _nambayaUserService.UpdateUser(user);
                return _nambayaUserService.GetUserById(user.ID);
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
        public ActionResult<Contracts.Models.UserBO> Delete(long id)
        {
            try
            {
                var user = _nambayaUserService.GetUserById(id);
                if (user == null)
                {
                    return NotFound();
                }
                _nambayaUserService.DeleteUser(user);
                return user;
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
        [HttpPut("{action}")]
        public ActionResult UpdateSettings([FromBody] List<UserSettingBO> userSettings)
        {
            try
            {
                SessionContext sessionContext = Request.HttpContext.GetSessionContext();
                if (_nambayaUserService.GetUserByEmail(sessionContext.LoginName).PhoneVerified == false
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

        [HttpGet("notificationtemplatetypes")]
        public ActionResult<IEnumerable<LookupsBO>> GetNotificationTemplateTypes()
        {
            var ph = _notificationSetupService.GetNotificationTemplateTypes();
            if (ph == null)
            {
                return NotFound();
            }
            return Ok(ph);
        }
        [HttpGet("notificationeventtypes")]
        public ActionResult<IEnumerable<LookupsBO>> GetNotificationEventTypes()
        {
            var ph = _notificationSetupService.GetNotificationEventTypes();
            if (ph == null)
            {
                return NotFound();
            }
            return Ok(ph);
        }
        [HttpGet("notificationtemplates/{id}")]
        public ActionResult<PagedResults<NotificationTemplateBO>> GetNotificationTemplates(long id)
        {
            var ph = _notificationSetupService.GetNotificationTemplate(id);
            if (ph == null)
            {
                return NotFound("NO_VALID_NOTIFICATION_TEMPLATE");
            }
            return Ok(ph);
        }
        [HttpGet("notificationtemplates")]
        public ActionResult<PagedResults<NotificationTemplateBO>> GetNotificationTemplates(int offset = 0, int limit = 0, string orderby = null, string filter = null)
        {
            var ph = _notificationSetupService.GetNotificationTemplates(limit, offset, orderby, filter);
            if (ph == null)
            {
                return NotFound("NO_VALID_NOTIFICATION_TEMPLATE");
            }
            return Ok(ph);
        }
        [HttpGet("notificationtemplates/eventtype")]
        public ActionResult<PagedResults<NotificationTemplateBO>> GetNotificationTemplate(long eventtypeid, long templatetypeid)
        {
            var ph = _notificationSetupService.GetNotificationTemplate(eventtypeid, templatetypeid);
            if (ph == null)
            {
                return NotFound("NO_VALID_NOTIFICATION_TEMPLATE");
            }
            return Ok(ph);
        }
        [HttpPost("notificationtemplates")]
        public ActionResult<NotificationTemplateBO> CreateTemplate([FromBody] NotificationTemplateBO notificationTemplateBO)
        {
            try
            {
                return Ok(_notificationSetupService.AddTemplate(notificationTemplateBO));
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
        [HttpPut("notificationtemplates")]
        public ActionResult<NotificationTemplateBO> UpdateTemplate([FromBody] NotificationTemplateBO notificationTemplateBO)
        {
            try
            {
                return Ok(_notificationSetupService.UpdateTemplate(notificationTemplateBO));
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
        [HttpGet("notificationtypes/{eventtypeid}")]
        public ActionResult<IEnumerable<NotificationEventTypeBO>> GetNotificationTypesByEvent(long eventtypeid)
        {
            var ph = _notificationSetupService.GetNotificationTypesByEvent(eventtypeid);
            if (ph == null)
            {
                return NotFound("NO_VALID_NOTIFICATION_TYPE");
            }
            return Ok(ph);
        }
        [HttpGet("eventparams/{eventtypeid}")]
        public ActionResult<IEnumerable<NotificationEventParamBO>> GetNotificationParamsByEvent(long eventtypeid)
        {
            var ph = _notificationSetupService.GetNotificationParamsByEvent(eventtypeid);
            if (ph == null)
            {
                return NotFound("NO_VALID_NOTIFICATION_TYPE");
            }
            return Ok(ph);
        }
        [HttpGet("role/{applicationCode}")]
        public ActionResult<IEnumerable<RoleBO>> GetRoles(string applicationCode)
        {
            var ph = _nambayaUserService.GetRoles(applicationCode);
            if (ph == null)
            {
                return NotFound("NO_VALID_NOTIFICATION_TYPE");
            }
            return Ok(ph);
        }
        [HttpGet("dashboard/userstats")]
        public ActionResult<object> GetUsersStats()
        {
            try
            {
                return Ok(_nambayaUserService.GetStats());
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
        #region Phone Verification
        [HttpPost("generatephoneverification")]
        public ActionResult<object> GetToken(VerifyPhoneRequest req)
        {
            try
            {
                var v = _nambayaUserService.GeneratePhoneVerification(req);
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
        public ActionResult<UserBO> VerifyToken(VerifyPhoneOtpRequest req)
        {
            try
            {
                var v = _nambayaUserService.VerifyPhoneVerification(req);
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
        [HttpGet("{action}")]
        public ActionResult<string[]> GetTemplateAllowedDomains()
        {
            try
            {
                string[] allowedUserDomains = _configuration.GetSection("TemplateAllowedUserDomains").Get<string[]>();
                return Ok(allowedUserDomains);
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
        public ActionResult<string[]> GetNambayaUserAllowedDomains()
        {
            try
            {
                string[] allowedUserDomains = _configuration.GetSection("NambayaUserAllowedUserDomains").Get<string[]>();
                return Ok(allowedUserDomains);
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
                var ph = _nambayaUserService.GetUserById(sessionContext.AppUserID);
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
