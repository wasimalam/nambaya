using Common.DataAccess;
using Common.DataAccess.Models;
using System;

namespace Patient.Repository.Models
{
    public partial class Patient : BaseModel
    {
        #region Data Members
        private string _title;
        private string _prefix;
        private string _firstname;
        private string _lastname;
        private string _suffix;
        private string _pharmacypatientid;
        private long _pharmacyid;
        private DateTime _dateofbirth;
        private long _genderid;
        private string _insurancenumber;
        private string _street;
        private string _zipcode;
        private string _address;
        private string _county;
        private string _email;
        private string _phone;
        private bool _isactive;
        #endregion

        #region Properties
        public long PharmacyID
        {
            get { return _pharmacyid; }
            set
            {
                if (_pharmacyid != value)
                {
                    _pharmacyid = value;
                    SetField(ref _pharmacyid, value, "PharmacyID");
                }
            }
        }
        public string PharmacyPatientID
        {
            get { return _pharmacypatientid; }
            set
            {
                if (_pharmacypatientid != value)
                {
                    _pharmacypatientid = value;
                    SetField(ref _pharmacypatientid, value, "PharmacyPatientID");
                }
            }
        }
        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    SetField(ref _title, value, "Title");
                }
            }
        }
        public string Prefix
        {
            get { return _prefix; }
            set
            {
                if (_prefix != value)
                {
                    _prefix = value;
                    SetField(ref _prefix, value, "Prefix");
                }
            }
        }
        [DapperSecured(Length = 50, DbType = System.Data.DbType.String)]
        public string FirstName
        {
            get { return _firstname; }
            set
            {
                if (_firstname != value)
                {
                    _firstname = value;
                    SetField(ref _firstname, value, "FirstName");
                }
            }
        }
        [DapperSecured(Length = 50, DbType = System.Data.DbType.String)]
        public string LastName
        {
            get { return _lastname; }
            set
            {
                if (_lastname != value)
                {
                    _lastname = value;
                    SetField(ref _lastname, value, "LastName");
                }
            }
        }
        public string Suffix
        {
            get { return _suffix; }
            set
            {
                if (_suffix != value)
                {
                    _suffix = value;
                    SetField(ref _suffix, value, "Suffix");
                }
            }
        }
        public DateTime DateOfBirth
        {
            get { return _dateofbirth; }
            set
            {
                if (_dateofbirth != value)
                {
                    _dateofbirth = value;
                    SetField(ref _dateofbirth, value, "DateOfBirth");
                }
            }
        }
        public long GenderID
        {
            get { return _genderid; }
            set
            {
                if (_genderid != value)
                {
                    _genderid = value;
                    SetField(ref _genderid, value, "GenderID");
                }
            }
        }
        [DapperSecured(Length = 15, DbType = System.Data.DbType.String)]
        public string InsuranceNumber
        {
            get { return _insurancenumber; }
            set
            {
                if (_insurancenumber != value)
                {
                    _insurancenumber = value;
                    SetField(ref _insurancenumber, value, "InsuranceNumber");
                }
            }
        }
        public string Street
        {
            get { return _street; }
            set
            {
                if (_street != value)
                {
                    _street = value;
                    SetField(ref _street, value, "Street");
                }
            }
        }
        public string ZipCode
        {
            get { return _zipcode; }
            set
            {
                if (_zipcode != value)
                {
                    _zipcode = value;
                    SetField(ref _zipcode, value, "ZipCode");
                }
            }
        }
        public string Address
        {
            get { return _address; }
            set
            {
                if (_address != value)
                {
                    _address = value;
                    SetField(ref _address, value, "Address");
                }
            }
        }
        public string County
        {
            get { return _county; }
            set
            {
                if (_county != value)
                {
                    _county = value;
                    SetField(ref _county, value, "County");
                }
            }
        }
        [DapperSecured(Length = 80, DbType = System.Data.DbType.String)]
        public string Email
        {
            get { return _email; }
            set
            {
                if (_email != value)
                {
                    _email = value;
                    SetField(ref _email, value, "Email");
                }
            }
        }
        [DapperSecured(Length = 20, DbType = System.Data.DbType.String)]
        public string Phone
        {
            get { return _phone; }
            set
            {
                if (_phone != value)
                {
                    _phone = value;
                    SetField(ref _phone, value, "Phone");
                }
            }
        }
        public bool IsActive
        {
            get { return _isactive; }
            set
            {
                if (_isactive != value)
                {
                    _isactive = value;
                    SetField(ref _isactive, value, "IsActive");
                }
            }
        }
        #endregion
        [DapperIgnore]
        public long DeviceID { get; set; }
        [DapperIgnore]
        public long CaseID { get; set; }
        [DapperIgnore]
        public DateTime? CaseStartDate { get; set; }
        [DapperIgnore]
        public DateTime? CaseEndDate { get; set; }
        [DapperIgnore]
        public bool CaseIsActive { get; set; }
        [DapperIgnore]
        public long StatusID { get; set; }
        [DapperIgnore]
        public long StepID { get; set; }
        [DapperIgnore]
        public long? CardiologistID { get; set; }
        [DapperIgnore]
        public long? DoctorID { get; set; }
        [DapperIgnore]
        public DateTime? EDFUploadDate { get; set; }
        [DapperIgnore]
        public long? QuickResultID { get; set; }
    }
}
