namespace Common.Shared
{
    public class PatientCaseDispatchMessage
    {
        public long PatientCaseId { get; set; }
        public string MedicationFilePath { get; set; }
        public string DetailEvaluationFilePath { get; set; }
        public string CreatedBy { get; set; }
    }
}
