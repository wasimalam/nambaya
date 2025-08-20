using Common.DataAccess;
using Common.DataAccess.Models;
using System;

namespace UserManagement.Repository.Models
{
    public class NotificationEventParam : BaseModel
    {
        #region Data Members
        private long _eventtypeid;
        private string _code;
        private bool _isactive;
        #endregion
        [DapperIgnore]
        public override string CreatedBy { get => base.CreatedBy; set => base.CreatedBy = value; }
        [DapperIgnore]
        public override string UpdatedBy { get => base.UpdatedBy; set => base.UpdatedBy = value; }
        [DapperIgnore]
        public override DateTime? CreatedOn { get => base.CreatedOn; set => base.CreatedOn = value; }
        [DapperIgnore]
        public override DateTime? UpdatedOn { get => base.UpdatedOn; set => base.UpdatedOn = value; }
        #region Properties
        public long EventTypeID
        {
            get { return _eventtypeid; }
            set
            {
                if (_eventtypeid != value)
                {
                    _eventtypeid = value;
                    SetField(ref _eventtypeid, value, "EventTypeID");
                }
            }
        }

        public string Code
        {
            get { return _code; }
            set
            {
                if (_code != value)
                {
                    _code = value;
                    SetField(ref _code, value, "Code");
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
    }
}
