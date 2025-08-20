using Common.Infrastructure;
using Common.Infrastructure.Exceptions;
using Identity.API.Authorities;
using Identity.API.Helpers;
using Identity.Contracts.Interfaces;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Identity.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class PasswordController : Controller
    {
        private IIdentityService _identityService;
        private readonly ILogger<PasswordController> _logger;
        private Dictionary<string, AuthorityIssuer> _resetPasswordIssuers;
        private readonly IServiceProvider _serviceProvider;

        public PasswordController(
            IIdentityService identityService,
            IServiceProvider serviceProvider,
            ILogger<PasswordController> logger)
        {
            _serviceProvider = serviceProvider;
            _identityService = identityService;
            _logger = logger;
            _resetPasswordIssuers = new Dictionary<string, AuthorityIssuer>()
            {
                {
                    "owner",
                    AuthorityIssuer.Create(new AuthenticationAuthority(), "identity")
                        .Register("account", new RPOTPAuthority(serviceProvider), AuthorityKeys.GetAuthorityKeys().rptokentimeout)
                        .Register("otp", new OTPAuthority(serviceProvider), AuthorityKeys.GetAuthorityKeys().passwordtokentimeout)
                        .Register("changepassword", new ChangePasswordAuthority(serviceProvider), AuthorityKeys.GetAuthorityKeys().rptokentimeout)
                }
            };
        }

        [HttpPost("account")]
        public async Task<IActionResult> Account([FromBody] AccountLoginRequest model)
        {
            return await Account("", model);
        }
        [HttpPost("account/{authority}")]
        public async Task<IActionResult> Account(string authority, [FromBody] AccountLoginRequest model)
        {
            if (model == null || model?.payload is System.Text.Json.JsonElement == false)
                return Unauthorized();
            var authorities = _resetPasswordIssuers["owner"].Authorities;
            if (!authorities.Any())
                return Unauthorized();
            string token = model.token;
            if (string.IsNullOrWhiteSpace(authority))
            {
                authority = authorities.Keys.ToArray()[0];
                token = JwtHelper.GenerateToken(new Claim[] { }, AuthorityKeys.GetAuthorityKeys().tokentimeout);
            }
            if (string.IsNullOrWhiteSpace(token))
                return Unauthorized();

            var principle = JwtHelper.GetClaimsPrincipal(token);
            if (principle?.Identity?.IsAuthenticated == true)
            {
                try
                {
                    var claimsIdentity = principle.Identity as ClaimsIdentity;
                    var verifyResult = _resetPasswordIssuers["owner"].Verify(authority, claimsIdentity.Claims.ToArray(), model.payload);
                    if (verifyResult.Authority == null)
                    {
                        long userid = 0;
                        long.TryParse(claimsIdentity.Claims.Single(c => c.Type == "identity").Value, out userid);
                        return await ChangePassword(userid, model.payload.GetProperty("password").ToString(), model.ReturnUrl);
                    }
                    return Ok(new { verify_token = verifyResult.Token, authority = verifyResult.Authority, parameters = verifyResult.Payload, expires_in = verifyResult.Expires_In });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    return Unauthorized(ex.Message);
                }
            }
            return Unauthorized();
        }
        private async Task<IActionResult> ChangePassword(long identity, string password, string returnUrl)
        {
            try
            {
                IIdentityServerInteractionService interaction = _serviceProvider.GetRequiredService<IIdentityServerInteractionService>();
                var context = (returnUrl != null) ? await interaction.GetAuthorizationContextAsync(returnUrl) : null;
                var user = _identityService.GetUMUser(identity);
                if (user == null)
                    return Unauthorized(ClientSideErrors.INVALID_USER_ID_PASSWORD);

                if (context != null && context.ScopesRequested.Contains(user.ApplicationCode))
                {
                    _identityService.ChangePassword(user.LoginName, password, "");
                    _logger.LogTrace($"User {user.UserID} found. Going to get App User details. ");
                    _identityService.GetUser(user);
                }
                if (user.AppUserID != 0 && context != null)
                {
                    var userSetting = _identityService.GetUserSetting(user.LoginName, UserSettingCodes.LANGUAGE);
                    var Claims = new List<Claim>()
                    {
                        new Claim(ClaimNames.ApplicationCode, user.ApplicationCode),
                        new Claim(ClaimNames.RoleCode, user.RoleCode),
                        new Claim(ClaimNames.AppUserID, user.AppUserID.ToString()),
                        new Claim(ClaimNames.Email, user.LoginName),
                        new Claim(ClaimNames.FirstName, user.FirstName?? " "),
                        new Claim(ClaimNames.LastName, user.LastName?? " "),
                        new Claim(ClaimNames.Name, user.FullName?? " "),
                        new Claim(ClaimNames.Language, userSetting?.Value?? LanguageCode.German),
                        new Claim(ClaimNames.PharmacyID, user.RoleCode==RoleCodes.Pharmacy?user.AppUserID.ToString():
                        (user.RoleCode==RoleCodes.Pharmacist?user.PharmacyID.ToString():"0")),
                        new Claim(ClaimNames.CardiologistID, user.RoleCode==RoleCodes.Cardiologist?user.AppUserID.ToString():
                        (user.RoleCode==RoleCodes.Nurse?user.CardiologistID.ToString():"0")),
                    };
                    await HttpContext.SignInAsync(user.UserID.ToString(), user.FullName, Claims.ToArray());
                    return new JsonResult(new { RedirectUrl = returnUrl, IsOk = true });
                }
            }
            catch (ServiceException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                if (ex.InnerException != null)
                {
                    _logger.LogTrace($"Inner Exception {ex.InnerException.Message} \n {ex.InnerException.StackTrace}");
                    return Unauthorized(ex.InnerException.Message);
                }
                else
                    return Unauthorized(ex.Message);
            }
            return Unauthorized();
        }
    }
    public enum OTPType
    {
        EMail = 1,
        SMS = 2,
        Both = 3
    }
}
