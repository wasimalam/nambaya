using Common.DataAccess.Models;

namespace Pharmacist.Repository.Models
{
    public partial class Device : BaseModel
    {
        #region Data Members
        long _pharmacyid;
        string _name;
        string _serialnumber;
        string _manufacturer;
        string _description;
        long _statusid;
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
        public string SerialNumber
        {
            get { return _serialnumber; }
            set
            {
                if (_serialnumber != value)
                {
                    _serialnumber = value;
                    SetField(ref _serialnumber, value, "SerialNumber");
                }
            }
        }
        public string Manufacturer
        {
            get { return _manufacturer; }
            set
            {
                if (_manufacturer != value)
                {
                    _manufacturer = value;
                    SetField(ref _manufacturer, value, "Manufacturer");
                }
            }
        }
        public string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    SetField(ref _description, value, "Description");
                }
            }
        }
        public long StatusID
        {
            get { return _statusid; }
            set
            {
                if (_statusid != value)
                {
                    _statusid = value;
                    SetField(ref _statusid, value, "StatusID");
                }
            }
        }
        #endregion
    }
}
