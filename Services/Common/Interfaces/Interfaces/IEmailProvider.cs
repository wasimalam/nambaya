using Common.Interfaces.Models;
using System.Collections.Generic;

namespace Common.Interfaces.Interfaces
{
    public interface IEmailProvider
    {
        bool Send(string address, string subject, string messageBody);
        bool Send(List<string> addresses, string subject, string messageBody);
        bool Send(List<string> addresses, string subject, string messageBody, List<EmailAttachment> attachments);
    }
}
