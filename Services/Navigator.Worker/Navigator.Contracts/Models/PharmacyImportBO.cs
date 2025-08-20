using CsvHelper.Configuration.Attributes;

namespace Navigator.Contracts.Models
{
    public class PharmacyImportBO
    {
        [Name("QT-Life")]
        public long QTLifeId { get; set; }
        [Name("Identifikation")]
        public string Identification { get; set; }
        [Name("Pharmaca")]
        public string Pharmaca { get; set; }
        [Name("Contact")]
        public string Contact { get; set; }
        [Name("Adress")]
        public string Address { get; set; }
        [Name("Zip Code")]
        public string ZipCode { get; set; }
        [Name("Citty")]
        public string City { get; set; }
        [Name("Phone")]
        public string Phone { get; set; }
        [Name("Fax")]
        public string Fax { get; set; }
        [Name("E-Mail")]
        public string Email { get; set; }
    }
}
