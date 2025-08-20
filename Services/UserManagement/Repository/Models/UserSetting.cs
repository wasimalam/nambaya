using Common.DataAccess.Models;

namespace UserManagement.Repository.Models
{
    public partial class UserSetting : BaseLookup
    {
        #region Data Members
        private long _userid;
        private long _applicationid;
        private string _value;
        private string _datatype;
        #endregion

        #region Properties
        public long UserId
        {
            get { return _userid; }
            set
            {
                if (_userid != value)
                {
                    _userid = value;
                    SetField(ref _userid, value, "UserId");
                }
            }
        }
        public long ApplicationID
        {
            get { return _applicationid; }
            set
            {
                if (_applicationid != value)
                {
                    _applicationid = value;
                    SetField(ref _applicationid, value, "ApplicationID");
                }
            }
        }
        public string Value
        {
            get { return _value; }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    SetField(ref _value, value, "Value");
                }
            }
        }
        public string DataType
        {
            get { return _datatype; }
            set
            {
                if (_datatype != value)
                {
                    _datatype = value;
                    SetField(ref _datatype, value, "DataType");
                }
            }
        }
        #endregion
    }
}
