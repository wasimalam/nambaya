using Common.DataAccess.Models;

namespace Pharmacist.Repository.Models
{
    public partial class Pharmacy : BaseModel
    {
        #region Data Members

        private string _name;
        private string _email;
        private string _identification;
        private string _contact;
        private string _fax;
        private string _phone;
        private bool _phoneverified;
        private string _street;
        private string _address;
        private string _zipcode;
        private string _county;

        #endregion

        #region Properties

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    SetField(ref _name, value, "Name");
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
        public string Identification
        {
            get { return _identification; }
            set
            {
                if (_identification != value)
                {
                    _identification = value;
                    SetField(ref _identification, value, "Identification");
                }
            }
        }
        public string Contact
        {
            get { return _contact; }
            set
            {
                if (_contact != value)
                {
                    _contact = value;
                    SetField(ref _contact, value, "Contact");
                }
            }
        }
        public string Fax
        {
            get { return _fax; }
            set
            {
                if (_fax != value)
                {
                    _fax = value;
                    SetField(ref _fax, value, "Fax");
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
        #endregion
    }
}
