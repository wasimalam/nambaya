using AutoMapper;
using Common.BusinessObjects;
using Common.Infrastructure;
using Common.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NambayaUser.Contracts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using UserManagement.Contracts.Models;

namespace NambayaUser.Service
{
    public class UserRegistrationNotificationService : BaseNotificationService, IUserRegistrationNotificationService, IPhoneVerificationNotificationService
    {
        private readonly IMapper _mapper;
        private readonly RabbitMQClient _rabbitMQClient;
        private readonly INambayaUserService _nambayaUserService;
        private readonly ILogger<UserRegistrationNotificationService> _logger;

        public UserRegistrationNotificationService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _nambayaUserService = serviceProvider.GetRequiredService<INambayaUserService>();
            _mapper = serviceProvider.GetRequiredService<IMapper>();
            _rabbitMQClient = serviceProvider.GetRequiredService<RabbitMQClient>();
            _logger = serviceProvider.GetRequiredService<ILogger<UserRegistrationNotificationService>>();
        }
        public void NotifyRegitration(BaseUserBO user)
        {
            _logger.LogInformation($"NotifyRegitration: user {Newtonsoft.Json.JsonConvert.SerializeObject(user)}");
            var emailTemplate = GetNotificationTemplate<NotificationTemplateBO>(NotificationEventType.UserRegisterEvent, NotificationTemplateType.Email);
            var smsTemplate = GetNotificationTemplate<NotificationTemplateBO>(NotificationEventType.UserRegisterEvent, NotificationTemplateType.SMS);
            var eventNotificationTypes = GetNotificationEvent<IEnumerable<NotificationEventTypeBO>>(NotificationEventType.UserRegisterEvent);
            string userphone = "";
            string username = "";

            BasicUser user1 = null;
            if (user.ApplicationCode == ApplicationNames.NamabayaUserApp)
                user1 = _mapper.Map<BasicUser>(_nambayaUserService.GetUserByEmail(user.LoginName));
            else
                user1 = GetAppUser(user);

            string useremail = user.LoginName;
            if (user1 != null)
            {
                username = user1.Name;
                if (string.IsNullOrWhiteSpace(username))
                    username = (user1.FirstName ?? "") + " " + (user1.LastName ?? "");
                userphone = user1.Phone ?? "";
            }
            else
            {
                _logger.LogInformation($"{nameof(NotifyRegitration)}: User is not found");
            }

            if (emailTemplate != null)
            {
                _logger.LogInformation($"{nameof(NotifyRegitration)}: About to send registration to {useremail}");
                _rabbitMQClient.SendMessage(KnownChannels.EMAIL_CHANNEL, new NotificationBO()
                {
                    EventTypeId = NotificationEventType.UserRegisterEvent,
                    LogIt = eventNotificationTypes.FirstOrDefault(p => p.NotificationTypeID == NotificationTemplateType.Email).LogIt,
                    Address = useremail,
                    Subject = FillUserPasswordTemplate(emailTemplate.Subject, username, userphone, useremail, user.Password)
                        .Replace(NotificationConstants.APPLICATIONURL, GetUserApplicationUIUrl(user.ApplicationCode)),
                    Body = FillUserPasswordTemplate(emailTemplate.Message, username, userphone, useremail, user.Password)
                        .Replace(NotificationConstants.APPLICATIONURL, GetUserApplicationUIUrl(user.ApplicationCode))
                });
            }
            else
            {
                _logger.LogInformation($"{nameof(NotifyRegitration)}: Email tempate not found");
            }
            if (smsTemplate != null && string.IsNullOrWhiteSpace(userphone) == false)
            {
                _rabbitMQClient.SendMessage(KnownChannels.SMS_CHANNEL, new NotificationBO()
                {
                    EventTypeId = NotificationEventType.UserRegisterEvent,
                    LogIt = eventNotificationTypes.FirstOrDefault(p => p.NotificationTypeID == NotificationTemplateType.SMS).LogIt,
                    Address = userphone,
                    Subject = FillUserPasswordTemplate(smsTemplate.Subject, username, userphone, useremail, user.Password),
                    Body = FillUserPasswordTemplate(smsTemplate.Message, username, userphone, useremail, user.Password)
                        .Replace(NotificationConstants.APPLICATIONURL, GetUserApplicationUIUrl(user.ApplicationCode))
                });
            }
            else
            {
                _logger.LogInformation($"{nameof(NotifyRegitration)}: Sms tempate not found");

            }
        }
        private BasicUser GetAppUser(BaseUserBO umUser)
        {
            _logger.LogInformation($"GetAppUser: Getting app user {umUser.ApplicationCode}");
            string webApiUrl;
            if (umUser.ApplicationCode == ApplicationNames.PharmacistApp)
                webApiUrl = _webApiConf.PharmacistServiceBaseUrl;
            else if (umUser.ApplicationCode == ApplicationNames.CardiologistApp)
                webApiUrl = _webApiConf.CardiologistServiceBaseUrl;
            else if (umUser.ApplicationCode == ApplicationNames.CentralGroupApp)
                webApiUrl = _webApiConf.CentralGroupServiceBaseUrl;
            else
                throw new NotImplementedException();
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, webApiUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/{umUser.Role}/email/" + umUser.LoginName).Result;
                    return JsonSerializer.Deserialize<BasicUser>(res, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }
        }
        public void NotifyPhoneVerification(UserOtp user)
        {
            var smsTemplate = GetNotificationTemplate<NotificationTemplateBO>(NotificationEventType.PhoneVerificationEvent, NotificationTemplateType.SMS);
            var eventNotificationTypes = GetNotificationEvent<IEnumerable<NotificationEventTypeBO>>(NotificationEventType.PhoneVerificationEvent);
            if (smsTemplate != null && string.IsNullOrWhiteSpace(user.Phone) == false)
            {
                if (smsTemplate != null)
                {
                    _logger.LogInformation($"Phone {user.Phone} verification message {smsTemplate.Message}");
                    _rabbitMQClient.SendMessage(KnownChannels.SMS_CHANNEL, new NotificationBO()
                    {
                        EventTypeId = NotificationEventType.PhoneVerificationEvent,
                        LogIt = eventNotificationTypes.FirstOrDefault(p => p.NotificationTypeID == NotificationTemplateType.SMS).LogIt,
                        Address = user.Phone,
                        Subject = FillUserOTPTemplate(smsTemplate.Subject, user.Name, user.Phone, user.Email, user.OTP),
                        Body = FillUserOTPTemplate(smsTemplate.Message, user.Name, user.Phone, user.Email, user.OTP)
                    });
                }
            }
        }
    }
    internal class BasicUser
    {
        public long ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
    }
}
