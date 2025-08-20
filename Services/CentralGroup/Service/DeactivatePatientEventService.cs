using CentralGroup.Contracts.Interfaces;
using Common.BusinessObjects;
using Common.Infrastructure;
using Common.Services;
using Microsoft.Extensions.DependencyInjection;
using Patient.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using UserManagement.Contracts.Models;

namespace CentralGroup.Service
{
    public class DeactivatePatientEventService : BaseNotificationService, IDeactivatePatientEventService
    {
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

        public DeactivatePatientEventService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _rabbitMQClient = serviceProvider.GetRequiredService<RabbitMQClient>();
        }
        public void DeactivatePatienOTPNotify(PatientUserOtp userOtp)
        {
            var emailTemplate = CreateTemplatedMessage(NotificationTemplateType.Email, NotificationEventType.PatientDeactivateEventPharma, userOtp.Patient);
            var smsTemplate = CreateTemplatedMessage(NotificationTemplateType.SMS, NotificationEventType.PatientDeactivateEventPharma, userOtp.Patient);
            var eventNotificationTypes = GetNotificationEvent<IEnumerable<NotificationEventTypeBO>>(NotificationEventType.PatientDeactivateEventPharma);
            var username = userOtp.Name;
            var userphone = userOtp.Phone;
            var useremail = userOtp.Email;

            if (smsTemplate != null && string.IsNullOrWhiteSpace(userphone) == false && userOtp.SMS)
            {
                _rabbitMQClient.SendMessage(KnownChannels.SMS_CHANNEL, new NotificationBO()
                {
                    EventTypeId = NotificationEventType.PatientDeactivateEventPharma,
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
                    EventTypeId = NotificationEventType.PatientDeactivateEventPharma,
                    LogIt = eventNotificationTypes.FirstOrDefault(p => p.NotificationTypeID == NotificationTemplateType.Email).LogIt,
                    Address = useremail,
                    Subject = FillUserOTPTemplate(emailTemplate.Subject, username, userphone, useremail, userOtp.OTP),
                    Body = FillUserOTPTemplate(emailTemplate.Message, username, userphone, useremail, userOtp.OTP)
                });
            }
        }

        private NotificationTemplateBO CreateTemplatedMessage(long templatetypeid, long eventtypeid, PatientBO patient)
        {
            var template = GetNotificationTemplate<NotificationTemplateBO>(eventtypeid, templatetypeid);
            if (string.IsNullOrWhiteSpace(template?.Subject) == false)
                template.Subject = FillPatientTemplate(template.Subject, patient);
            if (string.IsNullOrWhiteSpace(template?.Message) == false)
                template.Message = FillPatientTemplate(template.Message, patient);
            return template;
        }
    }
}
