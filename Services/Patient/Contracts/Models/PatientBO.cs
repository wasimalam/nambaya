using Common.BusinessObjects;
using Common.Infrastructure;
using System;
using System.Globalization;
using System.Xml.Serialization;

namespace Patient.Contracts.Models
{
    [XmlRoot("P")]
    public class PatientBO : BaseBO
    {
        private string _gender;
        private string _dateOfBirthString;

        public string PharmacyPatientID { get; set; }
        public long PharmacyID { get; set; }

        [XmlAttribute(AttributeName = "t")]
        public string Title { get; set; }

        [XmlAttribute(AttributeName = "v")]
        public string Prefix { get; set; }

        [XmlAttribute(AttributeName = "g")]
        public string FirstName { get; set; }

        [XmlAttribute(AttributeName = "f")]
        public string LastName { get; set; }

        [XmlAttribute(AttributeName = "z")]
        public string Suffix { get; set; }

        [XmlAttribute(AttributeName = "b")]
        public string DateOfBirthString
        {
            get => _dateOfBirthString;
            set
            {
                _dateOfBirthString = value;
                if (value != null)
                    DateOfBirth = DateTime.ParseExact(value, "yyyyMMdd", CultureInfo.InvariantCulture);
            }
        }

        public DateTime DateOfBirth { get; set; }

        [XmlAttribute(AttributeName = "s")]
        public string Gender
        {
            get => _gender;
            set
            {
                if (value == "M")
                {
                    GenderID = GenderCodes.Male;
                }
                else if (value == "W")
                {
                    GenderID = GenderCodes.Female;
                }
                else if (value == "X")
                {
                    GenderID = GenderCodes.Unknown;
                }
                else if (value == "D")
                {
                    GenderID = GenderCodes.Diverse;
                }

                _gender = value;
            }
        }

        public long GenderID { get; set; }

        [XmlAttribute(AttributeName = "egk")]
        public string InsuranceNumber { get; set; }

        public string Street { get; set; }
        public string ZipCode { get; set; }
        public string Address { get; set; }
        public string County { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }
        public long DeviceID { get; set; }

        public string Name
        {
            get { return FirstName + " " + LastName; }
        }

        public long CaseID { get; set; }

        public string CaseIDString
        {
            get
            {
                return CaseID.ToString("D6");
            }
        }
        public DateTime? CaseStartDate { get; set; }
        public DateTime? CaseEndDate { get; set; }
        public bool CaseIsActive { get; set; }
        public long StatusID { get; set; }
        public long StepID { get; set; }
        public long? CardiologistID { get; set; }
        public long? DoctorID { get; set; }
        public DateTime? EDFUploadDate { get; set; }
        public long? QuickResultID { get; set; }
    }
}
