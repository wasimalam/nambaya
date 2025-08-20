using Common.BusinessObjects;
using Common.Infrastructure;
using Common.Interfaces;
using Common.Interfaces.Interfaces;
using Common.Interfaces.Models;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notification.API.Consumers
{
    public class SendEmailCosumer : IConsumer<NotificationBO>
    {
        private ILogger<SendEmailCosumer> _logger;
        public static Dictionary<string, string> OverrideEmail = new Dictionary<string, string>();
        private readonly ILoggingService _loggingService;
        public IEmailProvider EmailService { get; private set; }
        public SendEmailCosumer(EmailProvider emailService, ILogger<SendEmailCosumer> logger, ILoggingService loggingService)
        {
            EmailService = emailService;
            _logger = logger;
            _loggingService = loggingService;
        }


        /// <summary>
        /// When a user is added in the database, an event is fired
        /// This function consume the event
        /// It sends an email to the newly created user to say welcome
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Consume(ConsumeContext<NotificationBO> context)
        {
          
            var attachments = context.Message.Attachments?.Select(p => new EmailAttachment()
            {
                Name = p.Name,
                ContentType = p.ContentType,
                Data = p.Data
            }).ToList();
            if (context.Message.Addresses != null)
            {
                _logger.LogInformation($"{nameof(SendEmailCosumer)}: Message received to send email to {context.Message.Addresses.ToArray()}");
                for (int i = 0; i < context.Message.Addresses.Count; i++)
                {
                    var addr = context.Message.Addresses[i];
                    if (OverrideEmail.ContainsKey(addr.ToLower()))
                    {
                        _logger.LogInformation($"Replacing {addr} with {OverrideEmail[addr.ToLower()]}");
                        context.Message.Addresses[i] = OverrideEmail[addr.ToLower()];
                    }
                }
            }
            else
            {
                _logger.LogInformation($"{nameof(SendEmailCosumer)}: Message received to send email to {context.Message.Address}");
                if (OverrideEmail.ContainsKey(context.Message.Address.ToLower()))
                {
                    _logger.LogInformation($"Replacing {context.Message.Address} with {OverrideEmail[context.Message.Address.ToLower()]}");
                    context.Message.Address = OverrideEmail[context.Message.Address.ToLower()];
                }
            }
            await Task.Run(() =>
            {
                EmailService.Send(context.Message.Addresses ?? new System.Collections.Generic.List<string> { context.Message.Address },
                    context.Message.Subject, context.Message.Body, attachments);
                if (context.Message.LogIt)
                    _loggingService.LogNotification(context.Message, NotificationTemplateType.Email);
            });
        }
    }
}
