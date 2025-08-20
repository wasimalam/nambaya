using Common.BusinessObjects;
using System;

namespace Pharmacist.Contracts.Models
{
    public class DeviceAssignmentBO : BaseBO
    {
        public long DeviceID { get; set; }
        public long PatientCaseID { get; set; }
        public DateTime AssignmentDate { get; set; }
        public bool IsAssigned { get; set; }
        public long DeviceStatusID { get; set; }
    }
}
