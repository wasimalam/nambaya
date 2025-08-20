using Common.BusinessObjects;
using Common.BusinessObjects.Interfaces;
using Common.Infrastructure;
using Common.Infrastructure.Exceptions;
using Common.Services.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pharmacist.Contracts.Interfaces;
using Pharmacist.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pharmacist.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class PharmacistController : ControllerBase
    {
        private readonly ILogger<PharmacistController> _logger;
        private IPharmacistService _pharmacistService;
        private readonly ICommonUserServce _commonUserService;
        public PharmacistController(ILogger<PharmacistController> logger, IPharmacistService pharma, ICommonUserServce commonUserService)
        {
            _logger = logger;
            _pharmacistService = pharma;
            _commonUserService = commonUserService;
        }
        [HttpGet("pharmacy")]
        public ActionResult<PagedResults<PharmacistBO>> Get(int offset = 0, int limit = 0, string orderby = null, string filter = null)
        {
            var ph = _pharmacistService.GetPharmacists(limit, offset, orderby, filter);
            if (ph == null)
            {
                return NotFound();
            }
            return Ok(ph);
        }
        [HttpGet("{id}")]
        public ActionResult<PharmacistBO> Get(int id)
        {
            var ph = _pharmacistService.GetPharmacistById(id);
            if (ph == null)
            {
                return NotFound();
            }
            return ph;
        }
        [HttpGet("email/{loginid}")]
        public ActionResult<PharmacistBO> Get(string loginid)
        {
            var ph = _pharmacistService.GetPharmacistByEmail(loginid);
            if (ph == null)
            {
                return NotFound();
            }
            return ph;
        }
        [HttpGet("{action}")]
        public ActionResult<string> GetPharmacyId()
        {
            SessionContext sessionContext = Request.HttpContext.GetSessionContext();
            if (sessionContext.RoleCode == RoleCodes.Pharmacist)
                return Ok(Common.Infrastructure.Helpers.Base36String.ToString((ulong)_pharmacistService.GetPharmacistById(sessionContext.AppUserID).PharmacyID).PadLeft(4, '0'));
            else if (sessionContext.RoleCode == RoleCodes.Pharmacy)
                return Ok(Common.Infrastructure.Helpers.Base36String.ToString((ulong)sessionContext.AppUserID).PadLeft(4, '0'));
            return BadRequest(ClientSideErrors.INVALID_USER_ACTION);
        }
        [HttpPost]
        public ActionResult<PharmacistBO> AddPharmacy(PharmacistBO pharmacist)
        {
            try
            {
                SessionContext sessionContext = Request.HttpContext.GetSessionContext();
                pharmacist.CreatedBy = sessionContext.LoginName;
                if (sessionContext.RoleCode == RoleCodes.Pharmacy)
                    pharmacist.PharmacyID = sessionContext.AppUserID;
                var id = _pharmacistService.AddPharmacist(pharmacist);
                return _pharmacistService.GetPharmacistById(id);
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
        public ActionResult<PharmacistBO> UpdatePharmacist(PharmacistBO pharmacist)
        {
            try
            {
                SessionContext sessionContext = Request.HttpContext.GetSessionContext();
                var ph = _pharmacistService.GetPharmacistById(pharmacist.ID);
                if (ph == null)
                {
                    return NotFound();
                }
                pharmacist.UpdatedBy = sessionContext.LoginName;
                if (sessionContext.RoleCode == RoleCodes.Pharmacy)
                    pharmacist.PharmacyID = sessionContext.AppUserID;
                _pharmacistService.UpdatePharmacist(pharmacist);
                return _pharmacistService.GetPharmacistById(pharmacist.ID);
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
        public ActionResult<PharmacistBO> DeletePharmacist(long id)
        {
            try
            {
                SessionContext sessionContext = Request.HttpContext.GetSessionContext();
                var ph = _pharmacistService.GetPharmacistById(id);
                if (ph == null)
                {
                    return NotFound();
                }
                _pharmacistService.DeletePharmacist(ph);
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
                if (_pharmacistService.GetPharmacistByEmail(sessionContext.LoginName).PhoneVerified == false
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
        #region Phone Verification
        [HttpPost("generatephoneverification")]
        public ActionResult<object> GetToken(VerifyPhoneRequest req)
        {
            try
            {
                var v = _pharmacistService.GeneratePhoneVerification(req);
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
        public ActionResult<PharmacistBO> VerifyToken(VerifyPhoneOtpRequest req)
        {
            try
            {
                var v = _pharmacistService.VerifyPhoneVerification(req);
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
                var ph = _pharmacistService.GetPharmacistById(sessionContext.AppUserID);
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
