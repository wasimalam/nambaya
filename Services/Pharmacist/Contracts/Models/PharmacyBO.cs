using Common.BusinessObjects;

namespace Pharmacist.Contracts.Models
{
    public class PharmacyBO : BaseUserBO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Identification { get; set; }
        public string Contact { get; set; }
        public string Fax { get; set; }
        public string Phone { get; set; }
		public bool PhoneVerified { get; set; }
        public string Street { get; set; }
        public string ZipCode { get; set; }
        public string Address { get; set; }
        public string County { get; set; }
    }
}
