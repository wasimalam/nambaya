using Common.Infrastructure;
using Common.Infrastructure.Exceptions;
using Identity.API.Authorities;
using Identity.API.Helpers;
using Identity.Contracts.Interfaces;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
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
    public class AuthenticateController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IWebHostEnvironment _environment;
        private readonly IEventService _events;
        private IIdentityService _identityService;
        private readonly ILogger<AuthenticateController> _logger;
        private Dictionary<string, AuthorityIssuer> _issuers;

        public AuthenticateController(
            IIdentityServerInteractionService interaction,
            IWebHostEnvironment environment,
            IEventService events,
            IIdentityService identityService,
            IServiceProvider serviceProvider,
            ILogger<AuthenticateController> logger)
        {
            _interaction = interaction;
            _environment = environment;
            _events = events;
            _identityService = identityService;
            _logger = logger;
            _issuers = new Dictionary<string, AuthorityIssuer>()
            {
                {
                    "owner",
                    AuthorityIssuer.Create(new AuthenticationAuthority(), "identity")
                        .Register("account", new AccountAuthority(serviceProvider), AuthorityKeys.GetAuthorityKeys().tokentimeout)
                        .Register("otp", new OTPAuthority(serviceProvider), AuthorityKeys.GetAuthorityKeys().tokentimeout)
                }
            };
        }

        public class LoginRequest
        {
            public string Email { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string ReturnUrl { get; set; }
        }

        //[HttpPost]
        /*public async Task<IActionResult> Login([FromBody]LoginRequest request)
        {
            try
            {
                var context = (request.ReturnUrl != null) ? await _interaction.GetAuthorizationContextAsync(request.ReturnUrl) : null;
                var user = _identityService.IsValidUser(request.Username, request.Password);
                if (user == null)
                    return Unauthorized(ClientSideErrors.INVALID_USER_ID_PASSWORD);
                if (context != null && context.ScopesRequested.Contains(user.ApplicationCode))
                {
                    _logger.LogTrace($"User {user.UserID} found. Going to get App User details. ");
                    _identityService.GetUser(user);
                }
                if (user.AppUserID != 0 && context != null)
                {
                    var claims = new List<Claim>()
                    {
                        new Claim(ClaimNames.ApplicationCode, user.ApplicationCode),
                        new Claim(ClaimNames.RoleCode, user.RoleCode),
                        new Claim(ClaimNames.AppUserID, user.AppUserID.ToString()),
                        new Claim(ClaimNames.Email, user.LoginName),
                        new Claim(ClaimNames.FirstName, user.FirstName?? " "),
                        new Claim(ClaimNames.LastName, user.LastName?? " "),
                        new Claim(ClaimNames.Name, user.FullName?? " ")
                    };
                    await HttpContext.SignInAsync(user.UserID.ToString(), user.FullName, claims.ToArray());
                    return new JsonResult(new { RedirectUrl = request.ReturnUrl, IsOk = true });
                }
            }
            catch (ServiceException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogTrace($"{ex.Message}\n{ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    _logger.LogTrace($"Inner Exception {ex.InnerException.Message} \n {ex.InnerException.StackTrace}");
                    return Unauthorized(ex.InnerException.Message);
                }
                else
                    return Unauthorized(ex.Message);
            }
            return Unauthorized();
        }*/
        private async Task<IActionResult> LoginByUserID(long identity, string returnUrl)
        {
            try
            {
                _logger.LogDebug($"LoginByUserID identity: {identity}, retururl: {returnUrl}");
                var context = (returnUrl != null) ? await _interaction.GetAuthorizationContextAsync(returnUrl) : null;
                var user = _identityService.GetUMUser(identity);
                if (user == null)
                    return Unauthorized(ClientSideErrors.INVALID_USER_ID_PASSWORD);
                if (context != null && context.ScopesRequested.Contains(user.ApplicationCode))
                {
                    _logger.LogDebug($"User {user.UserID} found. Going to get App User details. ");
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
            _logger.LogDebug($"{model.payload}, {model.token}, {model.ReturnUrl}");
            var authorities = _issuers["owner"].Authorities;
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
                    var verifyResult = _issuers["owner"].Verify(authority, claimsIdentity.Claims.ToArray(), model.payload);
                    Console.WriteLine($"Account {verifyResult}");

                    if (verifyResult.Authority == null)
                    {
                        Console.WriteLine($"Account {verifyResult.Authority}");
                        long userid = 0;
                        long.TryParse(claimsIdentity.Claims.Single(c => c.Type == "identity").Value, out userid);
                        return await LoginByUserID(userid, model.ReturnUrl);// Ok(new { auth_token = verifyResult.Token });
                    }
                    return Ok(new { verify_token = verifyResult.Token, authority = verifyResult.Authority, parameters = verifyResult.Payload });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    return Unauthorized(ex.Message);
                }
            }
            return Unauthorized();
        }

        [HttpGet]
        [Route("Logout")]
        public async Task<IActionResult> Logout(string logoutId)
        {
            var context = await _interaction.GetLogoutContextAsync(logoutId);
            bool showSignoutPrompt = true;

            if (context?.ShowSignoutPrompt == false)
            {
                // it's safe to automatically sign-out
                showSignoutPrompt = false;
            }

            if (User?.Identity.IsAuthenticated == true)
            {
                // delete local authentication cookie
                await HttpContext.SignOutAsync();
            }
            // no external signout supported for now (see \Quickstart\Account\AccountController.cs TriggerExternalSignout)
            return Ok(new
            {
                showSignoutPrompt,
                ClientName = string.IsNullOrEmpty(context?.ClientName) ? context?.ClientId : context?.ClientName,
                context?.PostLogoutRedirectUri,
                context?.SignOutIFrameUrl,
                logoutId
            });
        }

        [HttpGet]
        [Route("Error")]
        public async Task<IActionResult> Error(string errorId)
        {
            // retrieve error details from identityserver
            var message = await _interaction.GetErrorContextAsync(errorId);

            if (message != null)
            {
                if (!_environment.IsDevelopment())
                {
                    // only show in development
                    message.ErrorDescription = null;
                }
            }
            return Ok(message);
        }
    }

    public class AccountLoginRequest
    {
        public dynamic payload { get; set; }
        public string token { get; set; }
        public string ReturnUrl { get; set; }
    }
}
