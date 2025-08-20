using Common.DataAccess;
using Common.DataAccess.Models;
using System;

namespace UserManagement.Repository.Models
{
    public partial class PasswordResetLink : BaseModel
    {
        #region Data Members
        private long _userid;
        private string _linkguid;
        private string _link;
        private string _redirecturl;
        private int _expireafterminutes;
        #endregion
        [DapperIgnore]
        public override string CreatedBy { get => base.CreatedBy; set => base.CreatedBy = value; }
        [DapperIgnore]
        public override string UpdatedBy { get => base.UpdatedBy; set => base.UpdatedBy = value; }
        [DapperIgnore]
        public override DateTime? UpdatedOn { get => base.UpdatedOn; set => base.UpdatedOn = value; }

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
        public string LinkGUID
        {
            get { return _linkguid; }
            set
            {
                if (_linkguid != value)
                {
                    _linkguid = value;
                    SetField(ref _linkguid, value, "LinkGUID");
                }
            }
        }
        public string Link
        {
            get { return _link; }
            set
            {
                if (_link != value)
                {
                    _link = value;
                    SetField(ref _link, value, "Link");
                }
            }
        }
        public string RedirectUrl
        {
            get { return _redirecturl; }
            set
            {
                if (_redirecturl != value)
                {
                    _redirecturl = value;
                    SetField(ref _redirecturl, value, "RedirectUrl");
                }
            }
        }
        public int ExpireAfterMinutes
        {
            get { return _expireafterminutes; }
            set
            {
                if (_expireafterminutes != value)
                {
                    _expireafterminutes = value;
                    SetField(ref _expireafterminutes, value, "ExpireAfterMinutes");
                }
            }
        }
        #endregion
    }
}
