using Common.DataAccess;
using Common.DataAccess.Models;
using System;

namespace UserManagement.Repository.Models
{
    public partial class RolePermission : BaseModel
    {
        #region Data Members
        private long _roleid;
        private long _permissionid;
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
