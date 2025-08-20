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
    public class PharmacyController : ControllerBase
    {
        private readonly ILogger<PharmacyController> _logger;
        private IPharmacyService _pharmacyService;
        private readonly ICommonUserServce _commonUserService;

        public PharmacyController(ILogger<PharmacyController> logger, IPharmacyService pharma, ICommonUserServce commonUserService)
        {
            _logger = logger;
            _pharmacyService = pharma;
            _commonUserService = commonUserService;
        }
        [HttpGet("compact")]
        public ActionResult GetMinInfo(int offset = 0, int limit = 0, string orderby = null, string filter = null)
        {
            var ph = _pharmacyService.GetPharmacies(limit, offset, orderby, filter);
            if (ph == null)
            {
                return NotFound();
            }
            return Ok(ph.Data.Select(p => new { ID = p.ID, Name = p.Name }));
        }
        [HttpGet]
        public ActionResult<PagedResults<PharmacyBO>> Get(int offset = 0, int limit = 0, string orderby = null, string filter = null)
        {
            var ph = _pharmacyService.GetPharmacies(limit, offset, orderby, filter);
            if (ph == null)
            {
                return NotFound();
            }
            return Ok(ph);
        }
        [HttpGet("{id}")]
        public ActionResult<PharmacyBO> Get(int id)
        {
            var ph = _pharmacyService.GetPharmacyById(id);
            if (ph == null)
            {
                return NotFound();
            }
            return ph;
        }
        [HttpGet("{action}")]
        public ActionResult<string> GetPharmacyId()
        {
            try
            {
                return Ok(_pharmacyService.GetPharmacyIdentification());
            }
            catch (ServiceException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("total")]
        public ActionResult<long> GetTotal()
        {
            return _pharmacyService.GetTotal();
        }
        [HttpGet("email/{loginid}")]
        public ActionResult<PharmacyBO> Get(string loginid)
        {
            var ph = _pharmacyService.GetPharmacyByEmail(loginid);
            if (ph == null)
            {
                return NotFound();
            }
            return ph;
        }
        [HttpGet("pharmacist/{loginid}")]
        public ActionResult<PharmacyBO> GetPharmacyByPharmacist(string loginid)
        {
            var ph = _pharmacyService.GetPharmacyByPharmacist(loginid);
            if (ph == null)
            {
                return NotFound();
            }
            return ph;
        }
        [HttpPost]
        public ActionResult<PharmacyBO> AddPharmacy(PharmacyBO pharmacy)
        {
            try
            {
                SessionContext sessionContext = Request.HttpContext.GetSessionContext();
                pharmacy.CreatedBy = sessionContext.LoginName;
                if (sessionContext.RoleCode != RoleCodes.NambayaUser && sessionContext.RoleCode != RoleCodes.PharmacyTrainer
                    && sessionContext.RoleCode != RoleCodes.CentralGroupUser)
                    throw new ServiceException(ClientSideErrors.USER_NOT_AUTHORIZED);
                var id = _pharmacyService.AddPharmacy(pharmacy);
                return _pharmacyService.GetPharmacyById(id);
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
        public ActionResult<PharmacyBO> UpdatePharmacy(PharmacyBO pharmacy)
        {
            try
            {
                SessionContext sessionContext = Request.HttpContext.GetSessionContext();
                var ph = _pharmacyService.GetPharmacyById(pharmacy.ID);
                if (ph == null)
                {
                    return NotFound();
                }
                pharmacy.UpdatedBy = sessionContext.LoginName;
                _pharmacyService.UpdatePharmacy(pharmacy);
                return _pharmacyService.GetPharmacyById(pharmacy.ID);
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
        public ActionResult<PharmacyBO> DeletePharmacy(long id)
        {
            try
            {
                SessionContext sessionContext = Request.HttpContext.GetSessionContext();
                var ph = _pharmacyService.GetPharmacyById(id);
                if (ph == null)
                {
                    return NotFound();
                }
                _pharmacyService.DeletePharmacy(ph);
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
                if (_pharmacyService.GetPharmacyByEmail(sessionContext.LoginName).PhoneVerified == false
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
                var v = _pharmacyService.GeneratePhoneVerification(req);
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
        public ActionResult<PharmacyBO> VerifyToken(VerifyPhoneOtpRequest req)
        {
            try
            {
                var v = _pharmacyService.VerifyPhoneVerification(req);
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

        [HttpPost("changepassword")]
        public async Task<ActionResult<object>> ChangePassword(ChangeCredentialBO req)
        {
            try
            {
                SessionContext sessionContext = Request.HttpContext.GetSessionContext();
                req.UserId = sessionContext.UserID;
                var ph = _pharmacyService.GetPharmacyById(sessionContext.AppUserID);
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
    }
}
