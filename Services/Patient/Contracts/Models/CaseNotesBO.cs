using System;

namespace Patient.Contracts.Models
{
    public class CaseNotesBO
    {
        public long PatientCaseID { get; set; }
        public string RoleCode { get; set; }
        public string Notes { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
