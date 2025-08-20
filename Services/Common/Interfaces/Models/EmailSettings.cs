using System;

namespace Common.Interfaces.Models
{
    public class EmailSettings
    {
        public string SmtpHost { get; set; }

        public Int32 SmtpPort { get; set; }

        public string EmailAddress { get; set; }

        public string EmailPassword { get; set; }
    }
}
