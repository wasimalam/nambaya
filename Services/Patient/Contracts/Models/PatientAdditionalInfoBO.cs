using Common.BusinessObjects;
using System.Xml.Serialization;

namespace Patient.Contracts.Models
{
    [XmlRoot("O")]
    public class PatientAdditionalInfoBO : BaseBO
    {
        public long PatientCaseID { get; set; }

        [XmlAttribute(AttributeName = "ai")]
        public string AllergiesFreeText { get; set; }

        [XmlAttribute(AttributeName = "p")]
        public bool IsPregnant { get; set; }

        [XmlAttribute(AttributeName = "b")]
        public bool IsFeeding { get; set; }

        [XmlAttribute(AttributeName = "w")]
        public decimal Weight { get; set; }

        [XmlAttribute(AttributeName = "h")]
        public decimal Height { get; set; }

        [XmlAttribute(AttributeName = "c")]
        public decimal CreatinineValue { get; set; }

        [XmlAttribute(AttributeName = "x")]
        public string AdditionalFreeText { get; set; }
        public bool IsNurseCase { get; set; }
        public int? HealthStatus { get; set; }
    }
}
