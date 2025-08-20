using Common.BusinessObjects;
using Common.Infrastructure;
using Identity.Contracts.Interfaces;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;

namespace Identity.API.Authorities
{
    public class OTPAuthority : IAuthority
    {
        private readonly IIdentityService _identityService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<OTPAuthority> _logger;
        private readonly RabbitMQClient _rabbitMQClient;
        public OTPAuthority(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = _serviceProvider.GetRequiredService<ILogger<OTPAuthority>>();
            _identityService = _serviceProvider.GetRequiredService<IIdentityService>();
            _environment = _serviceProvider.GetRequiredService<IWebHostEnvironment>();
            _rabbitMQClient = _serviceProvider.GetRequiredService<RabbitMQClient>();
        }

        public string[] Payload => new string[] { "otp" };
        private Claim[] generateOTPClaims(string name, string phone, string email, string otpMethodType, string otppurpose)
        {
            var digit = 6;
            var numberformat = "######";
            var emailforotp = email;
            long notificationevent = NotificationEventType.LoginOTP;
            if (string.IsNullOrWhiteSpace(otppurpose) == false && otppurpose == "resetpassword")
            {
                notificationevent = NotificationEventType.ForgetPassword;
                digit = 8;
                numberformat = "########";
            }
            var otp = new Random().Next((int)Math.Pow(10, digit - 1) + 1, (int)Math.Pow(10, digit) - 1).ToString(numberformat);
            if (string.IsNullOrWhiteSpace(otpMethodType))
            {
                var settings = _identityService.GetUserSetting(email);
                if (settings.Value == NotificationType.Email.ToString())
                    otpMethodType = "email";
                else
                    otpMethodType = "sms";
            }
            if (_environment.IsDevelopment())
            {
                if (string.IsNullOrWhiteSpace(AuthorityKeys.GetAuthorityKeys().testOTP) == false)
                    otp = AuthorityKeys.GetAuthorityKeys().testOTP;
                if (string.IsNullOrWhiteSpace(AuthorityKeys.GetAuthorityKeys().testOTPEmail) == false)
                    emailforotp = AuthorityKeys.GetAuthorityKeys().testOTPEmail;
            }
            var logmsg = string.Format("Phone number {0} OTP is {1}", phone, otp);
            if (otpMethodType == "email")
            {
                logmsg = string.Format("Email {0} OTP is {1}", emailforotp, otp);
                var template = _identityService.GetNotificationTemplate(notificationevent, NotificationTemplateType.Email);
                var otpmessage = template?.Message ?? AuthorityKeys.GetAuthorityKeys().otpMessage;
                if (string.IsNullOrWhiteSpace(otpmessage))
                    otpmessage = $"OTP is {Common.Services.NotificationConstants.OTPCODE}";
                _rabbitMQClient.SendMessage(KnownChannels.EMAIL_CHANNEL, new NotificationBO()
                {
                    EventTypeId = notificationevent,
                    LogIt = false,
                    Address = emailforotp,
                    Subject = template?.Subject ?? "OTP",
                    Body = otpmessage.Replace(Common.Services.NotificationConstants.OTPCODE, otp).
                        Replace(Common.Services.NotificationConstants.NAME, name)
                });
            }
            else
            {
                var template = _identityService.GetNotificationTemplate(notificationevent, NotificationTemplateType.SMS);
                var otpmessage = template?.Message ?? AuthorityKeys.GetAuthorityKeys().otpMessage;
                if (string.IsNullOrWhiteSpace(otpmessage))
                    otpmessage = $"OTP is {Common.Services.NotificationConstants.OTPCODE}";
                _rabbitMQClient.SendMessage(KnownChannels.SMS_CHANNEL, new NotificationBO()
                {
                    EventTypeId = notificationevent,
                    LogIt = false,
                    Address = phone,
                    Body = otpmessage.Replace(Common.Services.NotificationConstants.OTPCODE, otp)
                        .Replace(Common.Services.NotificationConstants.NAME, name)
                });
            }
            _logger.LogInformation(string.Format("\n{0}\n{1}\n{0}\n", new String('*', logmsg.Length), logmsg));

            var sid = DateTime.Now.Ticks.ToString();

            var hash = string.Format("{0}:{1}", sid, otp).Sha256();
            return new Claim[]
            {
                new Claim("otp_id", sid),
                new Claim("otp_hash", hash),
                new Claim("otp_loginid", email),
                new Claim("otp_method", otpMethodType)
            };
        }

        public Claim[] OnForward(Claim[] claims)
        {
            var name = claims.FirstOrDefault(c => c.Type == "name").Value;
            var phone = claims.Single(c => c.Type == "phone").Value;
            var email = claims.Single(c => c.Type == "email").Value;
            var otptype = claims.SingleOrDefault(c => c.Type == "otptype")?.Value;
            var otppurpose = claims.SingleOrDefault(c => c.Type == "otppurpose")?.Value;

            return generateOTPClaims(name, phone, email, otptype, otppurpose);
        }

        public Claim[] OnVerify(Claim[] claims, JsonElement payload, string identifier, out bool valid)
        {
            valid = false;
            var id = claims.Single(c => c.Type == identifier).Value;
            var loginid = claims.SingleOrDefault(c => c.Type == "otp_loginid")?.Value;
            var otpId = claims.Single(c => c.Type == "otp_id").Value;
            var hash = claims.Single(c => c.Type == "otp_hash").Value;
            if (string.Format("{0}:{1}", otpId, payload.GetProperty("otp").ToString()).Sha256() == hash)
            {
                valid = true;
                return new Claim[]
                {
                    new Claim(identifier, id),
                    new Claim("otp_loginid", loginid)
                };
            }
            throw new ArgumentException();
        }
    }
}
