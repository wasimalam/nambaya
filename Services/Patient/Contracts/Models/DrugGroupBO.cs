using Common.BusinessObjects;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Patient.Contracts.Models
{
    [XmlRoot(ElementName = "S")]
    public class DrugGroupBO : BaseBO
    {
        private string _drugGroupCode;

        public long PatientCaseID { get; set; }

        [XmlAttribute(AttributeName = "c")]
        public string DrugGroupCode
        {
            get => _drugGroupCode;
            set
            {
                if (value == "411")
                {
                    DrugGroupCodeID = 411;
                }
                else if (value == "412")
                {
                    DrugGroupCodeID = 412;
                }
                else if (value == "413")
                {
                    DrugGroupCodeID = 413;
                }
                else if (value == "414")
                {
                    DrugGroupCodeID = 414;
                }
                else if (value == "415")
                {
                    DrugGroupCodeID = 415;
                }
                else if (value == "416")
                {
                    DrugGroupCodeID = 416;
                }
                else if (value == "417")
                {
                    DrugGroupCodeID = 417;
                }
                else if (value == "418")
                {
                    DrugGroupCodeID = 418;
                }
                else if (value == "419")
                {
                    DrugGroupCodeID = 419;
                }
                else if (value == "421")
                {
                    DrugGroupCodeID = 421;
                }
                else if (value == "422")
                {
                    DrugGroupCodeID = 422;
                }
                else if (value == "423")
                {
                    DrugGroupCodeID = 423;
                }
                else if (value == "424")
                {
                    DrugGroupCodeID = 424;
                }
                _drugGroupCode = value;
            }
        }

        public long? DrugGroupCodeID { get; set; }

        [XmlAttribute(AttributeName = "t")]
        public string DrugGroupName { get; set; }

        [XmlElement(ElementName = "M")]
        public List<DrugDetailsBO> DrugDetailsList { get; set; } = new List<DrugDetailsBO>();

        [XmlElement(ElementName = "X")]
        public List<DrugFreeTextBO> DrugFreeTextList { get; set; } = new List<DrugFreeTextBO>();

        [XmlElement(ElementName = "R")]
        public List<DrugReceipeBO> DrugReceipeList { get; set; } = new List<DrugReceipeBO>();
    }
}