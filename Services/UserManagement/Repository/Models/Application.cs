using Common.DataAccess.Models;

namespace UserManagement.Repository.Models
{
    public partial class Application : BaseLookup
    {
        #region Data Members
        private long _parentapplicationid;
        private bool _isactive;
        private bool _isdeleted;
        #endregion

        #region Properties
        public long ParentApplicationID
        {
            get { return _parentapplicationid; }
            set
            {
                if (_parentapplicationid != value)
                {
                    _parentapplicationid = value;
                    SetField(ref _parentapplicationid, value, "ParentApplicationID");
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
