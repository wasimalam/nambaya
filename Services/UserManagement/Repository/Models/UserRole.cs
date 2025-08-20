using Common.DataAccess;
using Common.DataAccess.Models;
using System;

namespace UserManagement.Repository.Models
{
    public partial class UserRole : BaseModel
    {
        #region Data Members
        private long _userid;
        private long _roleid;
        #endregion
        #region dapper ignore
        [DapperIgnore]
        public override string CreatedBy { get => base.CreatedBy; set => base.CreatedBy = value; }
        [DapperIgnore]
        public override string UpdatedBy { get => base.UpdatedBy; set => base.UpdatedBy = value; }
        [DapperIgnore]
        public override DateTime? CreatedOn { get => base.CreatedOn; set => base.CreatedOn = value; }
        [DapperIgnore]
        public override DateTime? UpdatedOn { get => base.UpdatedOn; set => base.UpdatedOn = value; }
        #endregion
        #region Properties
        public long UserID
        {
            get { return _userid; }
            set
            {
                if (_userid != value)
                {
                    _userid = value;
                    SetField(ref _userid, value, "UserID");
                }
            }
        }
        public long RoleID
        {
            get { return _roleid; }
            set
            {
                if (_roleid != value)
                {
                    _roleid = value;
                    SetField(ref _roleid, value, "RoleID");
                }
            }
        }
        #endregion
    }
}
