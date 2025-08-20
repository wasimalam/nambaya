using System;

namespace Logging.Contracts.Models
{
	public class NotificationsBO
	{
		public long Id { get; set; }
		public string Subject { get; set; }
		public string Message { get; set; }
		public string Recipient { get; set; }
		public long EventTypeId { get; set; }
		public long TemplateTypeId { get; set; }
		public DateTime TimeStamp { get; set; }
	}
}
