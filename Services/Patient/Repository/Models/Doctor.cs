using Common.DataAccess.Models;

namespace Patient.Repository.Models
{
    public partial class Doctor : BaseModel
    {
        #region Data Members
        private string _firstname;
        private string _lastname;
        private string _doctorid;
        private string _companyid;
        private string _street;
        private string _zipcode;
        private string _address;
        private string _county;
        private string _email;
        private string _phone;
        #endregion

        #region Properties

        public string DoctorID
        {
            get { return _doctorid; }
            set
            {
                if (_doctorid != value)
                {
                    _doctorid = value;
                    SetField(ref _doctorid, value, "DoctorID");
                }
            }
        }
        public string CompanyID
        {
            get { return _companyid; }
            set
            {
                if (_companyid != value)
                {
                    _companyid = value;
                    SetField(ref _companyid, value, "CompanyID");
                }
            }
        }
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
        #endregion
    }
}
