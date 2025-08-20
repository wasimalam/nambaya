using Common.BusinessObjects;

namespace Pharmacist.Contracts.Models
{
    public class PharmacistBO : BaseBO
    {
        public long PharmacyID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Degree { get; set; }
        public string Street { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
    }
}
