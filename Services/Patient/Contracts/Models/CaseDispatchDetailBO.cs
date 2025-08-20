using Common.BusinessObjects;
using System;

namespace Patient.Contracts.Models
{
    public class CaseDispatchDetailBO : BaseBO
    {
        public long PatientCaseID { get; set; }
        public DateTime DispatchDate { get; set; }
        public bool IsMedicationPlanAttached { get; set; }
        public bool IsDetailEvaluationAttached { get; set; }
    }
}
