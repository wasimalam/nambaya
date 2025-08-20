using Common.BusinessObjects;

namespace Patient.Contracts.Models
{
    public class DeviceReminderPayLoadBO
    {
        public string CorrelationId { get; set; }
        public PatientBO PatientBO { get; set; }
        public SessionContext SessionContext { get; set; }
        public DeviceAssignmentBO DeviceAssignment { get; set; }
    }
}
