using System;

namespace Common.Interfaces.Models
{
    public class EmailAttachment
    {
        public string Name { get; set; }

        public string ContentType { get; set; }

        public byte[] Data { get; set; }
    }
}
