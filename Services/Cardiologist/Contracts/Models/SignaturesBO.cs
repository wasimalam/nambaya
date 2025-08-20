using Common.BusinessObjects;

namespace Cardiologist.Contracts.Models
{
	public class SignaturesBO : BaseBO
	{
		public long CardiologistID { get; set; }
		public string FilePath { get; set; }
		public string FileName { get; set; }
		public long FileLength { get; set; }
		public string ContentType { get; set; }
		public byte[] FileData { get; set; }
		public string FileDataString { get; set; }
	}
}
