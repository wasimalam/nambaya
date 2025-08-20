using Common.BusinessObjects;

namespace Patient.Contracts.Models
{
    public class QEResultEventPayloadBO
    {
        public SessionContext SessionContext { get; set; }
        public PatientBO PatientBO { get; set; }
    }
}
