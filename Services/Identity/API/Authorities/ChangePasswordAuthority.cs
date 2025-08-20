using Identity.Contracts.Interfaces;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;

namespace Identity.API.Authorities
{
    public class ChangePasswordAuthority : IAuthority
    {
        private readonly IIdentityService _identityService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<OTPAuthority> _logger;
        public ChangePasswordAuthority(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = _serviceProvider.GetRequiredService<ILogger<OTPAuthority>>();
            _identityService = _serviceProvider.GetRequiredService<IIdentityService>();
            _environment = _serviceProvider.GetRequiredService<IWebHostEnvironment>();
        }

        public string[] Payload => new string[] { "changepassword" };
        private Claim[] generateClaims(string loginid)
        {
            var sid = DateTime.Now.Ticks.ToString();

            var hash = string.Format("{0}:{1}", sid, loginid.ToLower()).Sha256();
            return new Claim[]
            {
                new Claim("otp_id", sid),
                new Claim("otp_hash", hash)
            };
        }

        public Claim[] OnForward(Claim[] claims)
        {
            var loginid = claims.Single(c => c.Type == "otp_loginid")?.Value;
            return generateClaims(loginid);
        }

        public Claim[] OnVerify(Claim[] claims, JsonElement payload, string identifier, out bool valid)
        {
            valid = false;
            var id = claims.Single(c => c.Type == identifier).Value;
            var otpId = claims.Single(c => c.Type == "otp_id").Value;
            var hash = claims.Single(c => c.Type == "otp_hash").Value;
            var loginId = payload.GetProperty("loginid").ToString().ToLower();
            if (string.Format("{0}:{1}", otpId, loginId).Sha256() == hash)
            {
                valid = true;
                return new Claim[]
                {
                    new Claim(identifier, id)
                };
            }
            throw new ArgumentException();
        }
    }
}
