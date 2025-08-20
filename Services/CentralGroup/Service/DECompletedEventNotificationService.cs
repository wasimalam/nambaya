using CentralGroup.Contracts.Interfaces;
using Common.BusinessObjects;
using Common.Infrastructure;
using Common.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Patient.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Cardiologist.Contracts.Models;
using Microsoft.Extensions.Logging;
using UserManagement.Contracts.Models;

namespace CentralGroup.Service
{
    public class DECompletedEventNotificationService : BaseNotificationService, IDECompletedEventNotificationService
    {
        private readonly ICentralGroupService _centralGroupService;
        private readonly RabbitMQClient _rabbitMQClient;
        private readonly ILogger<DECompletedEventNotificationService> _logger;
        private Func<string, string, string, string, string> FillCardioTemplate = (message, username, userphone, useremail) =>
        {
            return message.Replace(NotificationConstants.CARDIOLOGISTNAME, username ?? "")
               .Replace(NotificationConstants.CARDIOLOGISTPHONE, userphone ?? "")
               .Replace(NotificationConstants.CARDIOLOGISTEMAIL, useremail);
        };
        private Func<string, PatientBO, string> FillPatientTemplate = (message, patient) =>
        {
            DateTime zeroTime = new DateTime(1, 1, 1);
            TimeSpan span = (DateTime.Now - patient.DateOfBirth);
            int years = (zeroTime + span).Year - 1;
            return message.Replace(NotificationConstants.PATIENTNAME, patient.Name).Replace(NotificationConstants.PATIENTID, patient.PharmacyPatientID)
               .Replace(NotificationConstants.PATIENTAGE, years.ToString()).Replace(NotificationConstants.PATIENTCASEID, patient.CaseIDString)
               .Replace(NotificationConstants.PATIENTINSURANCENUMBER, patient.InsuranceNumber)
               .Replace(NotificationConstants.DECOLOR, GetColorByQuickResultId(patient.QuickResultID));
        };

        private static string GetColorByQuickResultId(long? quickResultId)
        {
            return quickResultId != null ?
                Enum.GetName(typeof(QuickEvaluationResultEnum), quickResultId)
                : string.Empty;
        }
        public DECompletedEventNotificationService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<DECompletedEventNotificationService>>();
            _centralGroupService = serviceProvider.GetRequiredService<ICentralGroupService>();
            _rabbitMQClient = serviceProvider.GetRequiredService<RabbitMQClient>();
        }
        public void Notify(DECompletedEventPayloadBO payloadBO)
        {
            switch (payloadBO.PatientBO.StatusID)
            {
                case CaseStatus.DetailEvalCompleted:
                    NotifyCenterUsers(payloadBO.PatientBO);
                    NotifyDoctorAssociation(payloadBO);
                    break;
                case CaseStatus.DetailEvalUploaded:
                    NotifyToCardiologist(payloadBO);
                    break;
            }
        }

        private void NotifyToCardiologist(DECompletedEventPayloadBO payloadBO)
        {
            try
            {
                _logger.LogInformation($"{nameof(NotifyToCardiologist)}: Notify to cardiologist started");
                if (payloadBO.SessionContext.RoleCode != RoleCodes.Nurse) return;

                var user = GetCardiologistByNurseId(payloadBO.SessionContext.AppUserID);
                var nurseName = payloadBO.SessionContext.FullName;

                if (user == null || user.IsActive == false)
                {
                    _logger.LogInformation($"{nameof(NotifyToCardiologist)}: user does not exist or inactive.");
                    return;
                }

                _logger.LogInformation($"{nameof(NotifyToCardiologist)}: creating templates for sms/email");

                var emailTemplate = CreateTemplatedMessage(NotificationTemplateType.Email, NotificationEventType.DECompletionEventByNurseCardiologist, payloadBO.PatientBO);
                var smsTemplate = CreateTemplatedMessage(NotificationTemplateType.SMS, NotificationEventType.DECompletionEventByNurseCardiologist, payloadBO.PatientBO);
                var eventNotificationTypes = GetNotificationEvent<IEnumerable<NotificationEventTypeBO>>(NotificationEventType.DECompletionEventByNurseCardiologist);


                var username = (user.FirstName ?? "") + " " + (user.LastName ?? "");
                var userphone = user.Phone ?? "";
                var useremail = user.Email;
                if (emailTemplate != null)
                {
                    emailTemplate.Message = emailTemplate.Message.Replace(NotificationConstants.NURSENAME, nurseName);
                    _rabbitMQClient.SendMessage(KnownChannels.EMAIL_CHANNEL, new NotificationBO()
                    {
                        EventTypeId = NotificationEventType.DECompletedCenterUsers,
                        LogIt = eventNotificationTypes.FirstOrDefault(p => p.NotificationTypeID == NotificationTemplateType.Email).LogIt,
                        Address = useremail,
                        Subject = FillUserTemplate(emailTemplate.Subject, username, userphone, useremail),
                        Body = FillUserTemplate(emailTemplate.Message, username, userphone, useremail)
                    });
                }
                if (smsTemplate != null && string.IsNullOrWhiteSpace(userphone) == false)
                {
                    smsTemplate.Message = smsTemplate.Message.Replace(NotificationConstants.NURSENAME, nurseName);
                    _rabbitMQClient.SendMessage(KnownChannels.SMS_CHANNEL, new NotificationBO()
                    {
                        EventTypeId = NotificationEventType.DECompletedCenterUsers,
                        LogIt = eventNotificationTypes.FirstOrDefault(p => p.NotificationTypeID == NotificationTemplateType.SMS).LogIt,
                        Address = userphone,
                        Subject = FillUserTemplate(smsTemplate.Subject, username, userphone, useremail),
                        Body = FillUserTemplate(smsTemplate.Message, username, userphone, useremail)
                    });
                }
                _logger.LogInformation($"{nameof(NotifyToCardiologist)}: Notify to cardiologist completed");

            }
            catch (Exception e)
            {
                _logger.LogError(e, $"{nameof(NotifyToCardiologist)}: Failed to notify cardiologist :{e.Message}");
            }

        }

        private CardiologistBO GetCardiologistByNurseId(long id)
        {
            try
            {
                _logger.LogInformation($"Get cardiologist by nurse id {id}");
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.CardiologistServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/cardiologist/GetCardiologistByNurseId/{id}").Result;
                    return JsonSerializer.Deserialize<CardiologistBO>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch
            {
                return null;
            }
        }

        public void NotifyCenterUsers(PatientBO patient)
        {
            _logger.LogInformation("Notify center user started");
            var emailTemplate = CreateTemplatedMessage(NotificationTemplateType.Email, NotificationEventType.DECompletedCenterUsers, patient);
            var smsTemplate = CreateTemplatedMessage(NotificationTemplateType.SMS, NotificationEventType.DECompletedCenterUsers, patient);
            var eventNotificationTypes = GetNotificationEvent<IEnumerable<NotificationEventTypeBO>>(NotificationEventType.DECompletedCenterUsers);
            var users = _centralGroupService.GetCentralGroup(0, 0, "", "");
            foreach (var user1 in users.Data)
            {
                if (user1.IsActive == false)
                    continue;
                var username = (user1.FirstName ?? "") + " " + (user1.LastName ?? "");
                var userphone = user1.Phone ?? "";
                var useremail = user1.Email;
                if (emailTemplate != null)
                {
                    _rabbitMQClient.SendMessage(KnownChannels.EMAIL_CHANNEL, new NotificationBO()
                    {
                        EventTypeId = NotificationEventType.DECompletedCenterUsers,
                        LogIt = eventNotificationTypes.FirstOrDefault(p => p.NotificationTypeID == NotificationTemplateType.Email).LogIt,
                        Address = useremail,
                        Subject = FillUserTemplate(emailTemplate.Subject, username, userphone, useremail),
                        Body = FillUserTemplate(emailTemplate.Message, username, userphone, useremail)
                    });
                }
                if (smsTemplate != null && string.IsNullOrWhiteSpace(userphone) == false)
                {
                    _rabbitMQClient.SendMessage(KnownChannels.SMS_CHANNEL, new NotificationBO()
                    {
                        EventTypeId = NotificationEventType.DECompletedCenterUsers,
                        LogIt = eventNotificationTypes.FirstOrDefault(p => p.NotificationTypeID == NotificationTemplateType.SMS).LogIt,
                        Address = userphone,
                        Subject = FillUserTemplate(smsTemplate.Subject, username, userphone, useremail),
                        Body = FillUserTemplate(smsTemplate.Message, username, userphone, useremail)
                    });
                }
            }
        }
        public void NotifyDoctorAssociation(DECompletedEventPayloadBO payloadBO)
        {
            _logger.LogInformation($"Notify doctor association started");
            var patient = payloadBO.PatientBO;
            _logger.LogInformation($"Creating template for notification doctor association");
            var emailTemplate = CreateTemplatedMessage(NotificationTemplateType.Email, NotificationEventType.DECompletionEventDoctorAssociation, patient);
            var eventNotificationTypes = GetNotificationEvent<IEnumerable<NotificationEventTypeBO>>(NotificationEventType.DECompletionEventDoctorAssociation);

            var conf = _serviceProvider.GetRequiredService<IConfiguration>();
            var users = conf.GetSection(ConfigurationConsts.DoctorAssociationEmails).Get<string[]>();
            foreach (var emailAddress in users)
            {
                var username = (payloadBO.SessionContext.FirstName ?? "") + " " + (payloadBO.SessionContext.LastName ?? "");
                var userphone = payloadBO.SessionContext.Phone ?? "";
                var useremail = payloadBO.SessionContext.LoginName;
                if (emailTemplate != null)
                {
                    _rabbitMQClient.SendMessage(KnownChannels.EMAIL_CHANNEL, new NotificationBO()
                    {
                        EventTypeId = NotificationEventType.DECompletionEventDoctorAssociation,
                        LogIt = eventNotificationTypes.FirstOrDefault(p => p.NotificationTypeID == NotificationTemplateType.Email).LogIt,
                        Address = emailAddress,
                        Subject = FillCardioTemplate(emailTemplate.Subject, username, userphone, useremail),
                        Body = FillCardioTemplate(emailTemplate.Message, username, userphone, useremail)
                    });
                }
            }
        }
        public NotificationTemplateBO GetEmailNotificationTemplate(long eventtypeid)
        {
            return GetNotificationTemplate<NotificationTemplateBO>(eventtypeid, NotificationTemplateType.Email);
        }

        private NotificationTemplateBO CreateTemplatedMessage(long templatetypeid, long eventtypeid, PatientBO patient)
        {
            var template = GetNotificationTemplate<NotificationTemplateBO>(eventtypeid, templatetypeid);
            if (template?.Subject != null)
                template.Subject = FillPatientTemplate(template.Subject, patient);
            if (template?.Message != null)
                template.Message = FillPatientTemplate(template.Message, patient);
            return template;
        }
    }
}
