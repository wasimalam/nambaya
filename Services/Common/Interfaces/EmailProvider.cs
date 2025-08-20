using Common.Interfaces.Interfaces;
using Common.Interfaces.Models;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Interfaces
{
    public class EmailProvider : IEmailProvider
    {
        private ILogger<EmailProvider> _logger;
        public EmailSettings EmailSettings { get; private set; }

        public EmailProvider(EmailSettings emailSettings, ILogger<EmailProvider> logger)
        {
            EmailSettings = emailSettings;
            _logger = logger;
        }

        public bool Send(string emailAddress, string subject, string messageBody)
        {
            List<string> emailAddresses = new List<string> { emailAddress };
            return Send(emailAddresses, subject, messageBody);
        }
        public bool Send(List<string> emailAddresses, string subject, string messageBody)
        {
            return Send(emailAddresses, subject, messageBody, null);
        }
        public bool Send(List<string> emailAddresses, string subject, string messageBody, List<EmailAttachment> attachments)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(MailboxAddress.Parse(EmailSettings.EmailAddress));
                message.Subject = subject;
                //message.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = messageBody };
                var builder = new BodyBuilder { TextBody = messageBody };
                if (attachments != null)
                {
                    foreach (var attachment in attachments.ToList())
                    {
                        builder.Attachments.Add(attachment.Name, attachment.Data, ContentType.Parse(attachment.ContentType));
                    }
                }
                message.Body = builder.ToMessageBody();
                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    if (EmailSettings.SmtpPort > 0)
                        client.Connect(EmailSettings.SmtpHost, EmailSettings.SmtpPort, SecureSocketOptions.Auto);

                    client.Authenticate(EmailSettings.EmailAddress, EmailSettings.EmailPassword);

                    foreach (string emailAdress in emailAddresses)
                    {
                        message.To.Add(MailboxAddress.Parse(emailAdress));
                    }

                    client.Send(message);
                    _logger.LogInformation($"{nameof(EmailProvider.Send)}: Email has successfully sent to :{emailAddresses.ToArray()}");
                    client.Disconnect(true);
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Email sent to {string.Join(",", emailAddresses.ToArray())} failed with exception {ex.Message}");
                throw ex;
            }
        }
    }
}