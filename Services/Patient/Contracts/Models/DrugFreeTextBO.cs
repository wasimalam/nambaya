using Common.BusinessObjects;
using System.Xml.Serialization;

namespace Patient.Contracts.Models
{
    [XmlRoot(ElementName = "X")]
    public class DrugFreeTextBO : BaseBO
    {
        public long DrugGroupID { get; set; }

        [XmlAttribute(AttributeName = "t")]
        public string FreeTextInput { get; set; }
    }
}
