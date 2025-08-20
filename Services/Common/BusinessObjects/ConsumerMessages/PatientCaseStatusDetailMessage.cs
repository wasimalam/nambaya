namespace Common.BusinessObjects.ConsumerMessages
{
    public class PatientCaseStatusDetailMessage
    {
        public long PatientCaseId { get; set; }
        public bool IsMedicationFileAttached { get; set; }
        public bool IsDetailEvaluationFileAttached { get; set; }
        public bool IsSuccess { get; set; }
        public string CreatedBy { get; set; }
    }
}
