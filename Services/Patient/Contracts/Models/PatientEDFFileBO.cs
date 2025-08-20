using Common.BusinessObjects;

namespace Patient.Contracts.Models
{
    public class PatientEDFFileBO : BaseBO
    {
        public long PatientCaseID { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public byte[] FileData { get; set; }
        public long FileLength { get; set; }
        public string ContentType { get; set; }
        public long? Duration { get; set; }
    }
}
