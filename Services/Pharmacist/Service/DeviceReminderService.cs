using Common.BusinessObjects;
using Common.Infrastructure;
using Common.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Patient.Contracts.Models;
using Pharmacist.Contracts.Interfaces;
using Pharmacist.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using UserManagement.Contracts.Models;

namespace Pharmacist.Service
{
    public class DeviceReminderService : BaseNotificationService, IDeviceReminderService
    {
        private readonly IPatientService _patientService;
        private readonly IDeviceService _deviceService;
        private readonly IPharmacyRepository _pharmacyRepository;
        private readonly IPharmacistRepository _pharmacistRepository;
        private readonly RabbitMQClient _rabbitMQClient;
        private readonly ILogger<DeviceReminderService> _logger;

        private Func<string, PatientBO, string> FillPatientTemplate = (message, patient) =>
        {
            DateTime zeroTime = new DateTime(1, 1, 1);
            TimeSpan span = (DateTime.Now - patient.DateOfBirth);
            int years = (zeroTime + span).Year - 1;
            return message.Replace(NotificationConstants.PATIENTNAME, patient.Name).Replace(NotificationConstants.PATIENTID, patient.PharmacyPatientID)
               .Replace(NotificationConstants.PATIENTAGE, years.ToString()).Replace(NotificationConstants.PATIENTCASEID, patient.CaseIDString)
               .Replace(NotificationConstants.PATIENTINSURANCENUMBER, patient.InsuranceNumber);
        };

        public DeviceReminderService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _patientService = serviceProvider.GetRequiredService<IPatientService>();
            _deviceService = serviceProvider.GetRequiredService<IDeviceService>();
            _pharmacyRepository = serviceProvider.GetRequiredService<IPharmacyRepository>();
            _pharmacistRepository = serviceProvider.GetRequiredService<IPharmacistRepository>();
            _rabbitMQClient = serviceProvider.GetRequiredService<RabbitMQClient>();
            _logger = serviceProvider.GetRequiredService<ILogger<DeviceReminderService>>();
        }
        public void RemindOfDevice(DeviceReminderPayLoadBO deviceReminderPayLoad)
        {
            _logger.LogInformation("Remind of device started");
            var patient = deviceReminderPayLoad.PatientBO;
            if (patient?.CreatedBy != null)
            {
                _logger.LogInformation("Generating templated for email and sms");
                var emailTemplate = CreateTemplatedMessage(NotificationTemplateType.Email, NotificationEventType.DeviceReminderEvent, patient);
                var smsTemplate = CreateTemplatedMessage(NotificationTemplateType.SMS, NotificationEventType.DeviceReminderEvent, patient);
                var eventNotificationTypes = GetNotificationEvent<IEnumerable<NotificationEventTypeBO>>(NotificationEventType.DeviceReminderEvent);
                string userphone = "";
                string useremail = "";
                string username = "";

                Thread.SetData(Thread.GetNamedDataSlot(LoggingConstants.CorrelationId), deviceReminderPayLoad.CorrelationId);
                Thread.SetData(Thread.GetNamedDataSlot("NambayaSession"), deviceReminderPayLoad.SessionContext);
                var patientBO = _patientService.GetPatientbyID(patient.ID);
                if (patientBO == null || patientBO.IsActive == false)
                {
                    _logger.LogInformation("RemindOfDevice: patient not found");
                    return;
                }
                var devAssignment = _deviceService.GetDeviceAssignmentByID(deviceReminderPayLoad.DeviceAssignment.ID);
                if (devAssignment.IsAssigned == false)
                {
                    _logger.LogInformation("RemindOfDevice: Device is not assigned");
                    return;
                }

                var device = _deviceService.GetDeviceById(devAssignment.DeviceID);
                var user1 = _pharmacistRepository.GetByEmail(deviceReminderPayLoad.SessionContext.LoginName);
                var user2 = _pharmacyRepository.GetByEmail(deviceReminderPayLoad.SessionContext.LoginName);
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
                        EventTypeId = NotificationEventType.DeviceReminderEvent,
                        LogIt = eventNotificationTypes.FirstOrDefault(p => p.NotificationTypeID == NotificationTemplateType.Email).LogIt,
                        Address = useremail,
                        Subject = FillUserTemplate(emailTemplate.Subject, username, userphone, useremail).Replace(NotificationConstants.DEVICESERIAL, device.SerialNumber),
                        Body = FillUserTemplate(emailTemplate.Message, username, userphone, useremail).Replace(NotificationConstants.DEVICESERIAL, device.SerialNumber)
                    });
                }
                if (smsTemplate != null && string.IsNullOrWhiteSpace(userphone) == false)
                {
                    _rabbitMQClient.SendMessage(KnownChannels.SMS_CHANNEL, new NotificationBO()
                    {
                        EventTypeId = NotificationEventType.DeviceReminderEvent,
                        LogIt = eventNotificationTypes.FirstOrDefault(p => p.NotificationTypeID == NotificationTemplateType.SMS).LogIt,
                        Address = userphone,
                        Subject = FillUserTemplate(smsTemplate.Subject, username, userphone, useremail).Replace(NotificationConstants.DEVICESERIAL, device.SerialNumber),
                        Body = FillUserTemplate(smsTemplate.Message, username, userphone, useremail).Replace(NotificationConstants.DEVICESERIAL, device.SerialNumber)
                    });
                }
            }
        }
        private NotificationTemplateBO CreateTemplatedMessage(long templatetypeid, long eventtypeid, PatientBO patient)
        {
            _logger.LogInformation($"CreateTemplatedMessage: templatetypeid {templatetypeid}, eventtypeid {eventtypeid}, patient {Newtonsoft.Json.JsonConvert.SerializeObject(patient)}");
            var template = GetNotificationTemplate<NotificationTemplateBO>(eventtypeid, templatetypeid);
            if (template?.Subject != null)
            {
                template.Subject = FillPatientTemplate(template.Subject, patient);
            }
            else {
                _logger.LogWarning("Template subject is null");
            }

            if (template?.Message != null)
                template.Message = FillPatientTemplate(template.Message, patient);
            return template;
        }
    }
}