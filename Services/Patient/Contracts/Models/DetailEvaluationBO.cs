using Common.BusinessObjects;

namespace Patient.Contracts.Models
{
    public class DetailEvaluationBO : BaseBO
    {
        public long PatientCaseID { get; set; }
        public string ResultsPath { get; set; }
        public string FileName { get; set; }
        public byte[] FileData { get; set; }
        public long FileLength { get; set; }
        public string ContentType { get; set; }
        public long? NotesTypeID { get; set; }
        public string Notes { get; set; }
        public bool IsSigned { get; set; }
    }
}
