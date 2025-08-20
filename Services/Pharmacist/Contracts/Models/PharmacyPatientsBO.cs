using Common.BusinessObjects;

namespace Pharmacist.Contracts.Models
{
    public class PharmacyPatientsBO : BaseBO
    {
        public int PharmacyID { get; set; }
        public int PatientID { get; set; }
    }
}
