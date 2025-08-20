namespace Common.BusinessObjects.ConsumerMessages
{
    public class PatientCaseDispatchMessage
    {
        public long PatientCaseId { get; set; }
        public bool IsMedicationFileAttached { get; set; }
        public bool IsDetailEvaluationFileAttached { get; set; }
        public string CreatedBy { get; set; }
    }
}
