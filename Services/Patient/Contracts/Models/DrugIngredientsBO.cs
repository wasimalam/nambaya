using Common.BusinessObjects;
using System.Xml.Serialization;

namespace Patient.Contracts.Models
{
    [XmlRoot(ElementName = "W")]
    public class DrugIngredientsBO : BaseBO
    {
        public long DrugDetailsID { get; set; }

        [XmlAttribute(AttributeName = "w")]
        public string ActiveIngredients { get; set; }

        [XmlAttribute(AttributeName = "s")]
        public string Strength { get; set; }
    }
}
