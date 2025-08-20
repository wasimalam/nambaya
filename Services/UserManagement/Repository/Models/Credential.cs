using Common.DataAccess;
using Common.DataAccess.Models;

namespace UserManagement.Repository.Models
{
    public partial class Credential : BaseModel
    {
        #region Data Members
        private long _userid;
        private string _password;
        private bool _isdeleted;
        #endregion
        
        [DapperIgnore]
        public override long ID { get => base.ID; set => base.ID = value; }

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

        public string Password
        {
            get { return _password; }
            set
            {
                if (_password != value)
                {
                    _password = value;
                    SetField(ref _password, value, "Password");
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
