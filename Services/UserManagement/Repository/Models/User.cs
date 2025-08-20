using Common.DataAccess;
using Common.DataAccess.Models;
using System;

namespace UserManagement.Repository.Models
{
    public partial class User : BaseModel
    {
        #region Data Members
        private string _loginname;
        private bool _isactive;
        private bool _isdomainuser;
        private bool _isWelcomeMessageRequired;
        private bool _ispasswordresetrequired;
        private bool _islocked;
        private int _passwordattempts;
        private bool _isdeleted;
        private DateTime? _lastloggedinon;
        private int _daystoexpire;
        #endregion        
        #region Properties
        public string LoginName
        {
            get { return _loginname; }
            set
            {
                if (_loginname != value)
                {
                    _loginname = value;
                    SetField(ref _loginname, value, "LoginName");
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
        public bool IsDomainUser
        {
            get { return _isdomainuser; }
            set
            {
                if (_isdomainuser != value)
                {
                    _isdomainuser = value;
                    SetField(ref _isdomainuser, value, "IsDomainUser");
                }
            }
        }
        public bool IsPasswordResetRequired
        {
            get { return _ispasswordresetrequired; }
            set
            {
                if (_ispasswordresetrequired != value)
                {
                    _ispasswordresetrequired = value;
                    SetField(ref _ispasswordresetrequired, value, "IsPasswordResetRequired");
                }
            }
        }
        public bool IsLocked
        {
            get { return _islocked; }
            set
            {
                if (_islocked != value)
                {
                    _islocked = value;
                    SetField(ref _islocked, value, "IsLocked");
                }
            }
        }
        public int PasswordAttempts
        {
            get { return _passwordattempts; }
            set
            {
                if (_passwordattempts != value)
                {
                    _passwordattempts = value;
                    SetField(ref _passwordattempts, value, "PasswordAttempts");
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
        public DateTime? LastLoggedInOn
        {
            get { return _lastloggedinon; }
            set
            {
                if (_lastloggedinon != value)
                {
                    _lastloggedinon = value;
                    SetField(ref _lastloggedinon, value, "LastLoggedInOn");
                }
            }
        }
        public int DaysToExpire
        {
            get { return _daystoexpire; }
            set
            {
                if (_daystoexpire != value)
                {
                    _daystoexpire = value;
                    SetField(ref _daystoexpire, value, "DaysToExpire");
                }
            }
        }

        public bool IsWelcomeMessageRequired
        {
            get { return _isWelcomeMessageRequired; }
            set
            {
                if (_isWelcomeMessageRequired != value)
                {
                    _isWelcomeMessageRequired = value;
                    SetField(ref _isWelcomeMessageRequired, value, "IsWelcomeMessageRequired");
                }
            }
        }
        [DapperIgnore]
        public long? RoleID { get; set; }
        [DapperIgnore]
        public string Role { get; set; }
        #endregion
    }
}
