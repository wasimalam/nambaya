using System;

namespace Logging.Contracts.Models
{
	public class SeriLogsBO
	{
		public long Id { get; set; }
		public string Message { get; set; }
		public string Level { get; set; }
		public string Exception { get; set; }
		public string ApplicationName { get; set; }
		public string CorrelationId { get; set; }
		public DateTime TimeStamp { get; set; }
	}
}
