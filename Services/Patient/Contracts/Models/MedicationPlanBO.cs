using Common.BusinessObjects;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Patient.Contracts.Models
{
    [XmlRoot("MP")]
    public class MedicationPlanBO : BaseBO
    {
        [XmlElement(ElementName = "P")]
        public PatientBO Patient { get; set; }

        [XmlElement(ElementName = "O")]
        public PatientAdditionalInfoBO PatientAdditionalInfo { get; set; }

        [XmlElement(ElementName = "S")]
        public List<DrugGroupBO> DrugGroups { get; set; } = new List<DrugGroupBO>();
    }
}