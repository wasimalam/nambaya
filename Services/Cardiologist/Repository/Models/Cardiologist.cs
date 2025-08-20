using Common.DataAccess.Models;

namespace Cardiologist.Repository.Models
{
    public class Cardiologist : BaseModel
    {
        #region Data Member
        private string _doctorid;
        private string _companyid;
        private string _fisrtname;
        private string _lastname;
        private string _address;
        private string _street;
        private string _zipcode;
        private string _phone;
        private bool _phoneverified;
        private string _county;
        private string _email;
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
            get { return _fisrtname; }
            set
            {
                if (_fisrtname != value)
                {
                    _fisrtname = value;
                    SetField(ref _fisrtname, value, "FirstName");
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
        public bool PhoneVerified
        {
            get { return _phoneverified; }
            set
            {
                if (_phoneverified != value)
                {
                    _phoneverified = value;
                    SetField(ref _phoneverified, value, "PhoneVerified");
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
        #endregion
    }
}
