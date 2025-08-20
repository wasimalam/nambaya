using Common.BusinessObjects;
using System;

namespace Patient.Contracts.Models
{
    public class PatientCaseBO : BaseBO
    {
        public long PatientID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
        public long StatusID { get; set; }
        public long StepID { get; set; }
        public long? CardiologistID { get; set; }
        public long? DoctorID { get; set; }
    }
}