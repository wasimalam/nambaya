using Common.BusinessObjects;

namespace Patient.Contracts.Models
{
    public class QuickEvaluationResultBO : BaseBO
    {
        public long PatientCaseID { get; set; }
        public string MeasurementTime { get; set; }
        public long QuickResultID { get; set; }
        public string Notes { get; set; }
    }
}
