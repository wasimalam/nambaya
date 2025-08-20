using AutoMapper;
using Common.BusinessObjects;
using Common.Infrastructure;
using Common.Services;
using Microsoft.Extensions.DependencyInjection;
using Patient.Contracts.Models;
using Pharmacist.Contracts.Interfaces;
using Pharmacist.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using UserManagement.Contracts.Models;

namespace Pharmacist.Service
{
    public class NotificationService : BaseNotificationService, IPharmacistNotificationService
    {
        private readonly ApiRequestConfiguration _apiSecretConf;
        private readonly IPharmacyRepository _pharmacyRepository;
        private readonly IPharmacistRepository _pharmacistRepository;
        private readonly IPharmacyService _pharmacistService;
        private readonly IMapper _mapper;
        private readonly ILogger<NotificationService> _logger;
        private readonly RabbitMQClient _rabbitMQClient;
        private Func<string, PatientBO, string> FillPatientTemplate = (message, patient) =>
        {
            DateTime zeroTime = new DateTime(1, 1, 1);
            TimeSpan span = (DateTime.Now - patient.DateOfBirth);
            int years = (zeroTime + span).Year - 1;
            return message.Replace(NotificationConstants.PATIENTNAME, patient.Name).Replace(NotificationConstants.PATIENTID, patient.PharmacyPatientID)
               .Replace(NotificationConstants.PATIENTAGE, years.ToString()).Replace(NotificationConstants.PATIENTCASEID, patient.CaseIDString)
               .Replace(NotificationConstants.PATIENTINSURANCENUMBER, patient.InsuranceNumber);
        };

        public NotificationService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _mapper = serviceProvider.GetRequiredService<IMapper>();
            _apiSecretConf = serviceProvider.GetRequiredService<ApiRequestConfiguration>();
            _pharmacyRepository = serviceProvider.GetRequiredService<IPharmacyRepository>();
            _pharmacistRepository = serviceProvider.GetRequiredService<IPharmacistRepository>();
            _pharmacistService = serviceProvider.GetRequiredService<IPharmacyService>();
            _rabbitMQClient = serviceProvider.GetRequiredService<RabbitMQClient>();
            _logger = serviceProvider.GetRequiredService<ILogger<NotificationService>>();
        }
        public void NotifyQuickEvaluation(PatientBO patient)
        {
            if (patient?.CreatedBy != null)
            {
                var emailTemplate = CreateTemplatedMessage(NotificationTemplateType.Email, NotificationEventType.AutoQECompletedPharmacist, patient);
                var smsTemplate = CreateTemplatedMessage(NotificationTemplateType.SMS, NotificationEventType.AutoQECompletedPharmacist, patient);
                var eventNotificationTypes = GetNotificationEvent<IEnumerable<NotificationEventTypeBO>>(NotificationEventType.AutoQECompletedPharmacist);
                string userphone = "";
                string useremail = "";
                string username = "";

                var user1 = _pharmacistRepository.GetByEmail(patient.CreatedBy);
                var user2 = _pharmacyRepository.GetByEmail(patient.CreatedBy);
                if (user1 != null)
                {
                    username = (user1.FirstName ?? "") + " " + (user1.LastName ?? "");
                    userphone = user1.Phone ?? "";
                    useremail = user1.Email;
                }
                else if (user2 != null)
                {
                    username = user2.Name ?? "";
                    userphone = user2.Phone ?? "";
                    useremail = user2.Email;
                }
                if (emailTemplate != null)
                {
                    _rabbitMQClient.SendMessage(KnownChannels.EMAIL_CHANNEL, new NotificationBO()
                    {
                        EventTypeId = NotificationEventType.AutoQECompletedPharmacist,
                        LogIt = eventNotificationTypes.FirstOrDefault(p => p.NotificationTypeID == NotificationTemplateType.Email).LogIt,
                        Address = useremail,
                        Subject = FillUserTemplate(emailTemplate.Subject, username, userphone, useremail),
                        Body = FillUserTemplate(emailTemplate.Message, username, userphone, useremail),
                    });
                }
                if (smsTemplate != null && string.IsNullOrWhiteSpace(userphone) == false)
                {
                    _rabbitMQClient.SendMessage(KnownChannels.SMS_CHANNEL, new NotificationBO()
                    {
                        EventTypeId = NotificationEventType.AutoQECompletedPharmacist,
                        LogIt = eventNotificationTypes.FirstOrDefault(p => p.NotificationTypeID == NotificationTemplateType.SMS).LogIt,
                        Address = userphone,
                        Subject = FillUserTemplate(smsTemplate.Subject, username, userphone, useremail),
                        Body = FillUserTemplate(smsTemplate.Message, username, userphone, useremail)
                    });
                }
            }
        }

        public void RemindChargingDevice()
        {

            var emailTemplate = GetNotificationTemplate<NotificationTemplateBO>(NotificationEventType.ReminderDeviceCharge,
                NotificationTemplateType.Email);
            var smsTemplate = GetNotificationTemplate<NotificationTemplateBO>(NotificationEventType.ReminderDeviceCharge,
                NotificationTemplateType.SMS);

            var eventNotificationTypes = GetNotificationEvent<IEnumerable<NotificationEventTypeBO>>(NotificationEventType.ReminderDeviceCharge);

            var users = _pharmacistService.GetAllPharamacists();
            _logger.LogInformation($"{nameof(RemindChargingDevice)}: Total Pharmacist:{users.TotalCount}");
            foreach (var user1 in users.Data)
            {
                try
                {
                    if (!user1.IsActive)
                    {
                        _logger.LogInformation($"{nameof(RemindChargingDevice)}: {user1.LoginName} is not active.");
                        continue;
                    }

                    var username = user1.Name ?? "";
                    var userphone = user1.Phone ?? "";
                    var useremail = user1.Email;

                    if (emailTemplate != null)
                    {
                        _logger.LogInformation($"{nameof(RemindChargingDevice)}: About to send reminder to :{user1.LoginName}.");
                        _rabbitMQClient.SendMessage(KnownChannels.EMAIL_CHANNEL, new NotificationBO()
                        {
                            EventTypeId = NotificationEventType.ReminderDeviceCharge,
                            LogIt = eventNotificationTypes
                                .FirstOrDefault(p => p.NotificationTypeID == NotificationTemplateType.Email).LogIt,
                            Address = useremail,
                            Subject = FillUserTemplate(emailTemplate.Subject, username, userphone, useremail),
                            Body = FillUserTemplate(emailTemplate.Message, username, userphone, useremail)
                        });
                    }
                    else
                    {
                        _logger.LogInformation($"{nameof(RemindChargingDevice)}: email template is not defined");
                    }


                    if (smsTemplate != null && string.IsNullOrWhiteSpace(userphone) == false)
                    {
                        _rabbitMQClient.SendMessage(KnownChannels.SMS_CHANNEL, new NotificationBO()
                        {
                            EventTypeId = NotificationEventType.ReminderDeviceCharge,
                            LogIt = eventNotificationTypes
                                .FirstOrDefault(p => p.NotificationTypeID == NotificationTemplateType.SMS).LogIt,
                            Address = userphone,
                            Subject = FillUserTemplate(smsTemplate.Subject, username, userphone, useremail),
                            Body = FillUserTemplate(smsTemplate.Message, username, userphone, useremail)
                        });
                    }
                    else
                    {
                        _logger.LogInformation($"{nameof(RemindChargingDevice)}: sms template is not defined");

                    }
                }
                catch (Exception e)
                {
                    _logger.LogError($"{nameof(RemindChargingDevice)}: Error occurred while sending reminder to user :{user1.LoginName}");
                }
               
            }

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