using Common.BusinessObjects;
using Common.Infrastructure;

namespace Patient.Contracts.Models
{
    public class DECompletedEventPayloadBO
    {
        public SessionContext SessionContext { get; set; }
        public PatientBO PatientBO { get; set; }
    }
    public class PatientUserOtp : UserOtp
    {
        public SessionContext SessionContext { get; set; }
        public PatientBO Patient { get; set; }
    }
}
