using System.Collections.Generic;

namespace Common.BusinessObjects
{
    public class NotificationBO
    {
        public long EventTypeId { get; set; }
        public bool LogIt { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string SentBy { get; set; }
        public string Address { get; set; }
        public List<NotificationAttachment> Attachments { get; set; }
        public List<string> Addresses { get; set; }
    }
    public class NotificationAttachment
    {
        public string Name { get; set; }
        public string ContentType { get; set; }
        public byte[] Data { get; set; }

    }
}
