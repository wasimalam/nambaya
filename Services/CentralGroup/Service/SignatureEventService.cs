using Cardiologist.Contracts.Models;
using CentralGroup.Contracts.Interfaces;
using Common.BusinessObjects;
using Common.Infrastructure;
using Common.Services;
using Microsoft.Extensions.DependencyInjection;
using Patient.Contracts.Models;
using System;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using UserManagement.Contracts.Models;

namespace CentralGroup.Service
{
    public class SignatureEventService : BaseNotificationService, ISignatureSaveEventService, ISignatureDeleteEventService, IDESignatureEventService
    {
        private readonly RabbitMQClient _rabbitMQClient;
        private readonly ILogger<SignatureEventService> _logger;

        private Func<string, PatientBO, string> FillPatientTemplate = (message, patient) =>
        {
            DateTime zeroTime = new DateTime(1, 1, 1);
            TimeSpan span = (DateTime.Now - patient.DateOfBirth);
            int years = (zeroTime + span).Year - 1;
            return message.Replace(NotificationConstants.PATIENTNAME, patient.Name).Replace(NotificationConstants.PATIENTID, patient.PharmacyPatientID)
               .Replace(NotificationConstants.PATIENTAGE, years.ToString()).Replace(NotificationConstants.PATIENTCASEID, patient.CaseIDString)
               .Replace(NotificationConstants.PATIENTINSURANCENUMBER, patient.InsuranceNumber);
        };

        public SignatureEventService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _rabbitMQClient = serviceProvider.GetRequiredService<RabbitMQClient>();
            _logger = serviceProvider.GetRequiredService<ILogger<SignatureEventService>>();
        }
        public void SignatureSaveOTPNotify(UserOtp userOtp)
        {
            _logger.LogInformation("SignatureSaveOTPNotify: Started");
            var emailTemplate = GetNotificationTemplate<NotificationTemplateBO>(NotificationEventType.SignatureSaveEventCardiologist, NotificationTemplateType.Email);
            var smsTemplate = GetNotificationTemplate<NotificationTemplateBO>(NotificationEventType.SignatureSaveEventCardiologist, NotificationTemplateType.SMS);
            var eventNotificationTypes = GetNotificationEvent<IEnumerable<NotificationEventTypeBO>>(NotificationEventType.SignatureSaveEventCardiologist);
            var cardio = GetCardiologist(userOtp.AppUserID);
            {
                if (cardio.IsActive == false)
                    return;
                var username = (cardio.FirstName ?? "") + " " + (cardio.LastName ?? "");
                var userphone = cardio.Phone ?? "";
                var useremail = cardio.Email;

                if (smsTemplate != null && string.IsNullOrWhiteSpace(userphone) == false && userOtp.SMS)
                {
                    _logger.LogInformation("SignatureSaveOTPNotify: sending message by sms channel");
                    _rabbitMQClient.SendMessage(KnownChannels.SMS_CHANNEL, new NotificationBO()
                    {
                        EventTypeId = NotificationEventType.SignatureSaveEventCardiologist,
                        LogIt = eventNotificationTypes.FirstOrDefault(p => p.NotificationTypeID == NotificationTemplateType.SMS).LogIt,
                        Address = userphone,
                        Subject = "",
                        Body = FillUserOTPTemplate(smsTemplate.Message, username, userphone, useremail, userOtp.OTP)
                    });
                }
                else if (emailTemplate != null)
                {
                    _logger.LogInformation("SignatureSaveOTPNotify: sending message by email channel");

                    _rabbitMQClient.SendMessage(KnownChannels.EMAIL_CHANNEL, new NotificationBO()
                    {
                        EventTypeId = NotificationEventType.SignatureSaveEventCardiologist,
                        LogIt = eventNotificationTypes.FirstOrDefault(p => p.NotificationTypeID == NotificationTemplateType.Email).LogIt,
                        Address = useremail,
                        Subject = FillUserOTPTemplate(emailTemplate.Subject, username, userphone, useremail, userOtp.OTP),
                        Body = FillUserOTPTemplate(emailTemplate.Message, username, userphone, useremail, userOtp.OTP)
                    });
                }
            }
        }
        public void SignatureDeleteOTPNotify(UserOtp userOtp)
        {
            _logger.LogInformation("SignatureDeleteOTPNotify: Started");

            var emailTemplate = GetNotificationTemplate<NotificationTemplateBO>(NotificationEventType.SignatureDeleteEventCardiologist, NotificationTemplateType.Email);
            var smsTemplate = GetNotificationTemplate<NotificationTemplateBO>(NotificationEventType.SignatureDeleteEventCardiologist, NotificationTemplateType.SMS);
            var eventNotificationTypes = GetNotificationEvent<IEnumerable<NotificationEventTypeBO>>(NotificationEventType.SignatureDeleteEventCardiologist);
            var cardio = GetCardiologist(userOtp.AppUserID);
            {
                if (cardio.IsActive == false)
                    return;
                var username = (cardio.FirstName ?? "") + " " + (cardio.LastName ?? "");
                var userphone = cardio.Phone ?? "";
                var useremail = cardio.Email;

                if (smsTemplate != null && string.IsNullOrWhiteSpace(userphone) == false && userOtp.SMS)
                {
                    _logger.LogInformation("SignatureDeleteOTPNotify: sending message by sms channel");

                    _rabbitMQClient.SendMessage(KnownChannels.SMS_CHANNEL, new NotificationBO()
                    {
                        EventTypeId = NotificationEventType.SignatureDeleteEventCardiologist,
                        LogIt = eventNotificationTypes.FirstOrDefault(p => p.NotificationTypeID == NotificationTemplateType.SMS).LogIt,
                        Address = userphone,
                        Subject = "",
                        Body = FillUserOTPTemplate(smsTemplate.Message, username, userphone, useremail, userOtp.OTP)
                    });
                }
                else if (emailTemplate != null)
                {
                    _logger.LogInformation("SignatureDeleteOTPNotify: sending message by email channel");

                    _rabbitMQClient.SendMessage(KnownChannels.EMAIL_CHANNEL, new NotificationBO()
                    {
                        EventTypeId = NotificationEventType.SignatureDeleteEventCardiologist,
                        LogIt = eventNotificationTypes.FirstOrDefault(p => p.NotificationTypeID == NotificationTemplateType.Email).LogIt,
                        Address = useremail,
                        Subject = FillUserOTPTemplate(emailTemplate.Subject, username, userphone, useremail, userOtp.OTP),
                        Body = FillUserOTPTemplate(emailTemplate.Message, username, userphone, useremail, userOtp.OTP)
                    });
                }
                _logger.LogInformation("SignatureDeleteOTPNotify: Completed");
            }
        }
        public void DESignatureOTPNotify(PatientUserOtp userOtp)
        {
            _logger.LogInformation("DESignatureOTPNotify: started");
            var emailTemplate = CreateTemplatedMessage(NotificationTemplateType.Email, NotificationEventType.DESignatureEventCardiologist, userOtp.Patient);
            var smsTemplate = CreateTemplatedMessage(NotificationTemplateType.SMS, NotificationEventType.DESignatureEventCardiologist, userOtp.Patient);
            var eventNotificationTypes = GetNotificationEvent<IEnumerable<NotificationEventTypeBO>>(NotificationEventType.DESignatureEventCardiologist);
            var cardio = GetCardiologist(userOtp.AppUserID);
            {
                if (cardio.IsActive == false)
                    return;
                var username = (cardio.FirstName ?? "") + " " + (cardio.LastName ?? "");
                var userphone = cardio.Phone ?? "";
                var useremail = cardio.Email;

                if (smsTemplate != null && string.IsNullOrWhiteSpace(userphone) == false && userOtp.SMS)
                {
                    _rabbitMQClient.SendMessage(KnownChannels.SMS_CHANNEL, new NotificationBO()
                    {
                        EventTypeId = NotificationEventType.DESignatureEventCardiologist,
                        LogIt = eventNotificationTypes.FirstOrDefault(p => p.NotificationTypeID == NotificationTemplateType.SMS).LogIt,
                        Address = userphone,
                        Subject = "",
                        Body = FillUserOTPTemplate(smsTemplate.Message, username, userphone, useremail, userOtp.OTP)
                    });
                }
                else if (emailTemplate != null)
                {
                    _rabbitMQClient.SendMessage(KnownChannels.EMAIL_CHANNEL, new NotificationBO()
                    {
                        EventTypeId = NotificationEventType.DESignatureEventCardiologist,
                        LogIt = eventNotificationTypes.FirstOrDefault(p => p.NotificationTypeID == NotificationTemplateType.Email).LogIt,
                        Address = useremail,
                        Subject = FillUserOTPTemplate(emailTemplate.Subject, username, userphone, useremail, userOtp.OTP),
                        Body = FillUserOTPTemplate(emailTemplate.Message, username, userphone, useremail, userOtp.OTP)
                    });
                }
            }
        }
        private CardiologistBO GetCardiologist(long carddiologistid)
        {
            _logger.LogInformation($"GetCardiologist: carddiologistid {carddiologistid}");
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.CardiologistServiceBaseUrl))
                {
                    _logger.LogInformation("GetCardiologist: calling cardiologist api");
                    var res = apiClient.InternalServiceGetAsync($"api/v1/cardiologist/{carddiologistid}").Result;
                    return JsonSerializer.Deserialize<CardiologistBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch
            {
                return null;
            }
        }
        private NotificationTemplateBO CreateTemplatedMessage(long templatetypeid, long eventtypeid, PatientBO patient)
        {
            _logger.LogInformation($"CreateTemplatedMessage: templatetypeid {templatetypeid} eventtypeid {eventtypeid} patient{Newtonsoft.Json.JsonConvert.SerializeObject(patient)}");
            var template = GetNotificationTemplate<NotificationTemplateBO>(eventtypeid, templatetypeid);
            if (string.IsNullOrWhiteSpace(template?.Subject) == false)
                template.Subject = FillPatientTemplate(template.Subject, patient);
            if (string.IsNullOrWhiteSpace(template?.Message) == false)
                template.Message = FillPatientTemplate(template.Message, patient);
            return template;
        }
    }
}
