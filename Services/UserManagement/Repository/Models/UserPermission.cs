using Common.DataAccess;
using Common.DataAccess.Models;
using System;

namespace UserManagement.Repository.Models
{
    public partial class UserPermission : BaseModel
    {
        #region Data Members
        private long _userid;
        private long _permissionid;
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
        [DapperKey]
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
        public long PermissionID
        {
            get { return _permissionid; }
            set
            {
                if (_permissionid != value)
                {
                    _permissionid = value;
                    SetField(ref _permissionid, value, "PermissionID");
                }
            }
        }
        #endregion
    }
}
