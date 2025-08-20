using Common.DataAccess.Models;

namespace Identity.Repository.Models 
{
    public class HostsResgistry : BaseModel
    {
        #region Data Members
        string _host;
        string _address;       
        #endregion

        #region Properties
        public string Host
        {
            get { return _host; }
            set
            {
                if (_host != value)
                {
                    _host = value;
                    SetField(ref _host, value, "Host");
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
        #endregion
    }
}
