using Common.BusinessObjects;

namespace Patient.Contracts.Models
{
    public class EdfFileUpdatePayLoadBO
    {
        public string CorrelationId { get; set; }
        public PatientBO PatientBO { get; set; }
        public SessionContext SessionContext { get; set; }
        public bool IsCleanup { get; set; }
    }
}
