using Common.BusinessObjects;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Patient.Contracts.Models
{
    [XmlRoot(ElementName = "M")]
    public class DrugDetailsBO : BaseBO
    {
        private string _dosageNight;
        private string _dosageEvening;
        private string _dosageNoon;
        private string _dosageMorning;
        private string _dosageopenScheme;
        public long DrugGroupID { get; set; }

        [XmlAttribute(AttributeName = "p")]
        public long PZN { get; set; }

        [XmlAttribute(AttributeName = "a")]
        public string MedicineName { get; set; }

        [XmlAttribute(AttributeName = "f")]
        public string DosageFormCode { get; set; }

        [XmlAttribute(AttributeName = "fd")]
        public string DosageForm { get; set; }

        [XmlAttribute(AttributeName = "m")]
        public string DosageMorning
        {
            get => _dosageMorning;
            set
            {
                _dosageMorning = value;
                if (string.IsNullOrWhiteSpace(value) == false)
                    IsDosageMorning = true;
                else
                    _dosageMorning = "0";
            }
        }

        public bool IsDosageMorning { get; set; }

        [XmlAttribute(AttributeName = "d")]
        public string DosageNoon
        {
            get => _dosageNoon;
            set
            {
                _dosageNoon = value;
                if (string.IsNullOrWhiteSpace(value) == false)
                    IsDosageNoon = true;
                else
                    _dosageNoon = "0";
            }
        }

        public bool IsDosageNoon { get; set; }

        [XmlAttribute(AttributeName = "v")]
        public string DosageEvening
        {
            get => _dosageEvening;
            set
            {
                _dosageEvening = value;
                if (string.IsNullOrWhiteSpace(value) == false)
                    IsDosageEvening = true;
                else
                    _dosageEvening = "0";
            }
        }

        public bool IsDosageEvening { get; set; }

        [XmlAttribute(AttributeName = "h")]
        public string DosageNight
        {
            get => _dosageNight;
            set
            {
                _dosageNight = value;
                if (string.IsNullOrWhiteSpace(value) == false)
                    IsDosageNight = true;
                else
                    _dosageNight = "0";
            }
        }

        public bool IsDosageNight { get; set; }

        [XmlAttribute(AttributeName = "t")]
        public string DosageopenScheme
        {
            get => _dosageopenScheme;
            set
            {
                //"2-1-0-0,5"
                _dosageopenScheme = value;
                if (string.IsNullOrWhiteSpace(value) == false)
                {
                    var splitArr = value.Split('-');
                    if (splitArr.Length > 0)
                        DosageMorning = splitArr[0];
                    if (splitArr.Length > 1)
                        DosageNoon = splitArr[1];
                    if (splitArr.Length > 2)
                        DosageEvening = splitArr[2];
                    if (splitArr.Length > 3)
                        DosageNight = splitArr[3];
                }
            }
        }

        //public string DosageopenScheme { get; set; }

        [XmlAttribute(AttributeName = "du")]
        public string DosageUnitCode { get; set; }

        [XmlAttribute(AttributeName = "dud")]
        public string DosageUnitText { get; set; }

        [XmlAttribute(AttributeName = "i")]
        public string Hints { get; set; }

        [XmlAttribute(AttributeName = "r")]
        public string TreatmentReason { get; set; }

        [XmlAttribute(AttributeName = "x")]
        public string AdditionalText { get; set; }
        
        public bool IsActive { get; set; }

        [XmlElement(ElementName = "W")]
        public List<DrugIngredientsBO> DrugIngredientsList { get; set; } = new List<DrugIngredientsBO>();
    }
}