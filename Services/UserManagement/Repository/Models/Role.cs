using Common.DataAccess.Models;

namespace UserManagement.Repository.Models
{
    public partial class Role : BaseLookup
    {
        #region Data Members
        private long _applicationid;
        private string _adgroup;
        private bool _isactive;
        private bool _isdeleted;
        #endregion

        #region Properties
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
        public string ADGroup
        {
            get { return _adgroup; }
            set
            {
                if (_adgroup != value)
                {
                    _adgroup = value;
                    SetField(ref _adgroup, value, "ADGroup");
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
        public bool IsDeleted
        {
            get { return _isdeleted; }
            set
            {
                if (_isdeleted != value)
                {
                    _isdeleted = value;
                    SetField(ref _isdeleted, value, "IsDeleted");
                }
            }
        }
        #endregion
    }
}
