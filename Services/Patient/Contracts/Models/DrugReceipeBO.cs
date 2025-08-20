using Common.BusinessObjects;
using System.Xml.Serialization;

namespace Patient.Contracts.Models
{
    [XmlRoot(ElementName = "R")]
    public class DrugReceipeBO : BaseBO
    {
        public long DrugGroupID { get; set; }

        [XmlAttribute(AttributeName = "t")]
        public string FormulationText { get; set; }

        [XmlAttribute(AttributeName = "x")]
        public string FormulationAdditionalText { get; set; }
    }
}
