using Cardiologist.Contracts.Interfaces;
using Cardiologist.Contracts.Models;
using Common.BusinessObjects;
using Common.BusinessObjects.Interfaces;
using Common.Infrastructure;
using Common.Infrastructure.Exceptions;
using Common.Services.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cardiologist.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class CardiologistController : ControllerBase
    {
        private readonly ILogger<CardiologistController> _logger;
        private readonly ICardiologistService _cardiologistService;
        private readonly ISignatureService _signatureService;
        private readonly ICommonUserServce _commonUserService;
        private readonly INurseService _nurseService;
        public CardiologistController(ILogger<CardiologistController> logger, ICardiologistService cardiologist,
            ISignatureService signatureService, ICommonUserServce commonUserService, INurseService nurseService)
        {
            _logger = logger;
            _cardiologistService = cardiologist;
            _commonUserService = commonUserService;
            _signatureService = signatureService;
            _nurseService = nurseService;
        }
        [HttpGet]
        public ActionResult<PagedResults<CardiologistBO>> Get(int offset = 0, int limit = 0, string orderby = null, string filter = null)
        {
            var ph = _cardiologistService.GetCardiologists(limit, offset, orderby, filter);
            if (ph == null)
            {
                return NotFound();
            }
            return Ok(ph);
        }
        [HttpGet("{id}")]
        public ActionResult<CardiologistBO> Get(long id)
        {
            var ph = _cardiologistService.GetCardiologistById(id);
            if (ph == null)
            {
                return NotFound();
            }
            return ph;
        }
        [HttpGet("total")]
        public ActionResult<long> GetTotal()
        {
            return Ok(_cardiologistService.GetTotal());
        }
        [HttpGet("email/{loginid}")]
        public ActionResult<CardiologistBO> Get(string loginid)
        {
            var ph = _cardiologistService.GetCardiologistByEmail(loginid);
            if (ph == null)
            {
                return NotFound();
            }
            return ph;
        }
        [HttpGet("{action}/{id}")]
        public ActionResult<CardiologistBO> GetCardiologistByNurseId(long id)
        {
            var nurse = _nurseService.GetNurseById(id);
            if (nurse == null)
            {
                return NotFound();
            }

            var cardiologist = _cardiologistService.GetCardiologistById(nurse.CardiologistID);

            if (cardiologist == null)
            {
                return NotFound();
            }

            return cardiologist;
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
                if (_cardiologistService.GetCardiologistByEmail(sessionContext.LoginName).PhoneVerified == false
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
        [HttpPost]
        public ActionResult<CardiologistBO> AddCardiologist(CardiologistBO cardiologist)
        {
            try
            {
                SessionContext sessionContext = Request.HttpContext.GetSessionContext();
                cardiologist.CreatedBy = sessionContext.LoginName;
                var id = _cardiologistService.AddCardiologist(cardiologist);
                return _cardiologistService.GetCardiologistById(id);
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
        public ActionResult<CardiologistBO> UpdateCardiologist(CardiologistBO cardiologist)
        {
            try
            {
                SessionContext sessionContext = Request.HttpContext.GetSessionContext();
                var ph = _cardiologistService.GetCardiologistById(cardiologist.ID);
                if (ph == null)
                {
                    return NotFound();
                }
                cardiologist.UpdatedBy = sessionContext.LoginName;
                _cardiologistService.UpdateCardiologist(cardiologist);
                return _cardiologistService.GetCardiologistById(cardiologist.ID);
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
        public ActionResult<CardiologistBO> DeleteCardiologist(long id)
        {
            try
            {
                SessionContext sessionContext = Request.HttpContext.GetSessionContext();
                var ph = _cardiologistService.GetCardiologistById(id);
                if (ph == null)
                {
                    return NotFound();
                }
                _cardiologistService.DeleteCardiologist(ph);
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
        #region Phone Verification
        [HttpPost("generatephoneverification")]
        public ActionResult<object> GetToken(VerifyPhoneRequest req)
        {
            try
            {
                var v = _cardiologistService.GeneratePhoneVerification(req);
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
        public ActionResult<CardiologistBO> VerifyToken(VerifyPhoneOtpRequest req)
        {
            try
            {
                var v = _cardiologistService.VerifyPhoneVerification(req);
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
                var ph = _cardiologistService.GetCardiologistById(sessionContext.AppUserID);
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

        #region Signatures
        [HttpGet("{action}"), DisableRequestSizeLimit]
        public ActionResult<SignaturesBO> GetSignaturesData()
        {
            SignaturesBO signatureData;
            try
            {
                signatureData = _signatureService.GetSignatures();
                if (signatureData == null)
                    return NotFound();
                return Ok(signatureData);
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
        [HttpGet("{action}")]
        public ActionResult<object> GetSaveSignaturesToken()
        {
            try
            {
                var v = _signatureService.GenerateSignatureSaveOTP();
                return Ok(new { token = v, ttl = 120 });
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
        public ActionResult UploadSignatureFile([FromForm] string req, IFormFile fileKey)
        {
            _logger.LogTrace($"UploadSignatureFile Enter");
            try
            {
                if (fileKey.Length > 0)
                {
                    UpdateSignatureOtpRequest updateSignatureOtpRequest =
                        JsonSerializer.Deserialize<UpdateSignatureOtpRequest>(req, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                    using (var stream = new MemoryStream())
                    {
                        fileKey.CopyTo(stream);
                        updateSignatureOtpRequest.ImageData = Convert.ToBase64String(stream.GetBuffer());
                        var res = _signatureService.VerifySignatureSave(updateSignatureOtpRequest);
                        return Ok(res);
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
            return BadRequest(ClientSideErrors.FAILED_TO_UPLOAD);
        }
        [HttpPost("{action}"), DisableRequestSizeLimit, RequestFormLimits(MultipartBodyLengthLimit = System.Int32.MaxValue, ValueLengthLimit = System.Int32.MaxValue)]
        public ActionResult UploadSignatureData(UpdateSignatureOtpRequest updateSignatureOtpRequest)
        {
            _logger.LogTrace($"UploadSignatureData Enter for {updateSignatureOtpRequest.Email}");
            try
            {
                var res = _signatureService.VerifySignatureSave(updateSignatureOtpRequest);
                return Ok(res);
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

        [HttpGet("{action}")]
        public ActionResult<object> GetDeleteSignaturesToken()
        {
            try
            {
                var v = _signatureService.GenerateSignatureDeleteOTP();
                return Ok(new { token = v, ttl = 120 });
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
        [HttpDelete("{action}")]
        public ActionResult DeleteSignatureData(UpdateSignatureOtpRequest updateSignatureOtpRequest)
        {
            _logger.LogTrace($"DeleteSignatureData Enter for {updateSignatureOtpRequest.Email}");
            try
            {
                _signatureService.VerifySignatureDelete(updateSignatureOtpRequest);
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
        #endregion
    }
}
