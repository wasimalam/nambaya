using Cardiologist.Contracts.Models;
using CentralGroup.Contracts.Interfaces;
using Common.BusinessObjects;
using Common.Infrastructure;
using Common.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Patient.Contracts.Models;
using Pharmacist.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using UserManagement.Contracts.Models;

namespace CentralGroup.Service
{
    public class QEResultEventNotificationService : BaseNotificationService, IQEResultEventNotificationService
    {
        private readonly RabbitMQClient _rabbitMQClient;
        private readonly ICentralGroupService _centralGroupService;
        private Func<string, PharmacyBO, string, string, string, string> FillPharmacyTemplate = (message, pharmacy, username, userphone, useremail) =>
        {
            return message.Replace(NotificationConstants.PHARMACISTNAME, username ?? "")
                        .Replace(NotificationConstants.PHARMACISTPHONE, userphone ?? "")
                        .Replace(NotificationConstants.PHARMACISTEMAIL, useremail)
                        .Replace(NotificationConstants.PHARMACYIDENTIFICATION, pharmacy.Identification)
                        .Replace(NotificationConstants.PHARMACYCONTACT, pharmacy.Contact)
                        .Replace(NotificationConstants.PHARMACYNAME, pharmacy.Name);
        };
        private Func<string, PatientBO, string> FillPatientTemplate = (message, patient) =>
        {
            DateTime zeroTime = new DateTime(1, 1, 1);
            TimeSpan span = (DateTime.Now - patient.DateOfBirth);
            int years = (zeroTime + span).Year - 1;
            return message.Replace(NotificationConstants.PATIENTNAME, patient.Name).Replace(NotificationConstants.PATIENTID, patient.PharmacyPatientID)
               .Replace(NotificationConstants.PATIENTAGE, years.ToString()).Replace(NotificationConstants.PATIENTCASEID, patient.CaseIDString)
               .Replace(NotificationConstants.PATIENTINSURANCENUMBER, patient.InsuranceNumber);
        };
        private readonly ILogger<QEResultEventNotificationService> _logger;
        public QEResultEventNotificationService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<QEResultEventNotificationService>>();
            _centralGroupService = serviceProvider.GetRequiredService<ICentralGroupService>();
            _rabbitMQClient = serviceProvider.GetRequiredService<RabbitMQClient>();
        }
        public void Notify(QEResultEventPayloadBO payloadBO)
        {
            _logger.LogInformation("Notifing all users");
            NotifyCardiologists(payloadBO.PatientBO);
            NotifyNurses(payloadBO.PatientBO);
            NotifyPharmaAssociation(payloadBO);
            NotifyCenterUsers(payloadBO.PatientBO);
        }
        public void NotifyNurses(PatientBO patient)
        {
            try
            {
                _logger.LogInformation($"NotifyNurses: Start create templates for email/sms");
                var emailTemplate = CreateTemplatedMessage(NotificationTemplateType.Email, NotificationEventType.QEReusltEventCardiologists, patient);
                var smsTemplate = CreateTemplatedMessage(NotificationTemplateType.SMS, NotificationEventType.QEReusltEventCardiologists, patient);
                var eventNotificationTypes = GetNotificationEvent<IEnumerable<NotificationEventTypeBO>>(NotificationEventType.QEReusltEventCardiologists);
                _logger.LogInformation("Getting all nurses");
                var users = GetNurses();
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
                            EventTypeId = NotificationEventType.QEReusltEventCardiologists,
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
                            EventTypeId = NotificationEventType.QEReusltEventCardiologists,
                            LogIt = eventNotificationTypes.FirstOrDefault(p => p.NotificationTypeID == NotificationTemplateType.SMS).LogIt,
                            Address = userphone,
                            Subject = "",
                            Body = FillUserTemplate(smsTemplate.Message, username, userphone, useremail)
                        });
                    }
                }
                _logger.LogInformation("NotifyNurses: completed");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to notify nurse:{e.Message}");
            }

        }
        public void NotifyCardiologists(PatientBO patient)
        {
            try
            {
                _logger.LogInformation("Notify Cardiologists started");
                var emailTemplate = CreateTemplatedMessage(NotificationTemplateType.Email, NotificationEventType.QEReusltEventCardiologists, patient);
                var smsTemplate = CreateTemplatedMessage(NotificationTemplateType.SMS, NotificationEventType.QEReusltEventCardiologists, patient);
                var eventNotificationTypes = GetNotificationEvent<IEnumerable<NotificationEventTypeBO>>(NotificationEventType.QEReusltEventCardiologists);

                _logger.LogInformation("Getting cardiologist data");
                var users = GetCardiologists();
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
                            EventTypeId = NotificationEventType.QEReusltEventCardiologists,
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
                            EventTypeId = NotificationEventType.QEReusltEventCardiologists,
                            LogIt = eventNotificationTypes.FirstOrDefault(p => p.NotificationTypeID == NotificationTemplateType.SMS).LogIt,
                            Address = userphone,
                            Subject = "",
                            Body = FillUserTemplate(smsTemplate.Message, username, userphone, useremail)
                        });
                    }
                }
                _logger.LogInformation("Notify Cardiologists completed");

            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to notify cardioogist:{e.Message}");
            }

        }
        public void NotifyPharmaAssociation(QEResultEventPayloadBO payloadBO)
        {
            try
            {
                _logger.LogInformation($"NotifyPharmaAssociation: started");
                var patient = payloadBO.PatientBO;
                _logger.LogInformation($"NotifyPharmaAssociation: creating templates for email/sms");

                var emailTemplate = CreateTemplatedMessage(NotificationTemplateType.Email, NotificationEventType.QEReusltEventPharmaAssociation, patient);
                var eventNotificationTypes = GetNotificationEvent<IEnumerable<NotificationEventTypeBO>>(NotificationEventType.QEReusltEventPharmaAssociation);
                var conf = _serviceProvider.GetRequiredService<IConfiguration>();
                var users = conf.GetSection(ConfigurationConsts.PharmaAssociationEmails).Get<string[]>();
                _logger.LogInformation("Getting all pharmacy data");
                var pharmacy = GetPharmacy(payloadBO.SessionContext.PharmacyID);
                foreach (var emailAddress in users)
                {
                    var username = (payloadBO.SessionContext.FirstName ?? "") + " " + (payloadBO.SessionContext.LastName ?? "");
                    var userphone = payloadBO.SessionContext.Phone ?? "";
                    var useremail = payloadBO.SessionContext.LoginName;
                    if (emailTemplate != null)
                    {
                        _rabbitMQClient.SendMessage(KnownChannels.EMAIL_CHANNEL, new NotificationBO()
                        {
                            EventTypeId = NotificationEventType.QEReusltEventPharmaAssociation,
                            LogIt = eventNotificationTypes.FirstOrDefault(p => p.NotificationTypeID == NotificationTemplateType.Email).LogIt,
                            Address = emailAddress,
                            Subject = FillPharmacyTemplate(emailTemplate.Subject, pharmacy, username, userphone, useremail),
                            Body = FillPharmacyTemplate(emailTemplate.Message, pharmacy, username, userphone, useremail)
                        });
                    }
                }
                _logger.LogInformation("NotifyPharmaAssociation: completed");

            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to notify pharma association:{e.Message}");
            }

        }
        public void NotifyCenterUsers(PatientBO patient)
        {
            try
            {
                _logger.LogInformation("NotifyCenterUsers started");
                var emailTemplate = CreateTemplatedMessage(NotificationTemplateType.Email, NotificationEventType.QEReusltEventCeneterUsers, patient);
                var smsTemplate = CreateTemplatedMessage(NotificationTemplateType.SMS, NotificationEventType.QEReusltEventCeneterUsers, patient);
                var eventNotificationTypes = GetNotificationEvent<IEnumerable<NotificationEventTypeBO>>(NotificationEventType.QEReusltEventCeneterUsers);
                _logger.LogInformation("Getting center user data");
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
                            EventTypeId = NotificationEventType.QEReusltEventCeneterUsers,
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
                            EventTypeId = NotificationEventType.QEReusltEventCeneterUsers,
                            LogIt = eventNotificationTypes.FirstOrDefault(p => p.NotificationTypeID == NotificationTemplateType.SMS).LogIt,
                            Address = userphone,
                            Subject = FillUserTemplate(smsTemplate.Subject, username, userphone, useremail),
                            Body = FillUserTemplate(smsTemplate.Message, username, userphone, useremail)
                        });
                    }
                }
                _logger.LogInformation("NotifyCenterUsers completed");

            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to notify central users:{e.Message}");
            }

        }
        private PagedResults<CardiologistBO> GetCardiologists()
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.CardiologistServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/cardiologist").Result;
                    return JsonSerializer.Deserialize<PagedResults<CardiologistBO>>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch
            {
                return null;
            }
        }

        private PagedResults<NurseBO> GetNurses()
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.CardiologistServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/nurse/getall").Result;
                    return JsonSerializer.Deserialize<PagedResults<NurseBO>>(res,
                        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                }
            }
            catch
            {
                return null;
            }
        }
        private PharmacyBO GetPharmacy(long pharmacyid)
        {
            try
            {
                using (WebApiClient apiClient = new WebApiClient(_serviceProvider, _webApiConf.PharmacistServiceBaseUrl))
                {
                    var res = apiClient.InternalServiceGetAsync($"api/v1/pharmacy/{pharmacyid}").Result;
                    return JsonSerializer.Deserialize<PharmacyBO>(res,
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
            _logger.LogInformation($"CreateTemplatedMessage templatetypeid {templatetypeid} eventtypeid {eventtypeid}");
            var template = GetNotificationTemplate<NotificationTemplateBO>(eventtypeid, templatetypeid);
            if (template?.Subject != null)
                template.Subject = FillPatientTemplate(template.Subject, patient);
            if (template?.Message != null)
                template.Message = FillPatientTemplate(template.Message, patient);
            return template;
        }
    }
}
