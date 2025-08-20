using System;

namespace Common.BusinessObjects
{
    public class SessionContext
    {
        public long UserID { get; set; }
        public long AppUserID { get; set; }
        public long PharmacyID { get; set; }
        public long CardiologistID { get; set; }
        public string LoginName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }
        public bool IsPasswordResetRequired { get; set; }
        public bool IsLocked { get; set; }
        public int PasswordAttempts { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? LastLoggedInOn { get; set; }
        public int DaysToExpire { get; set; }
        public string RoleCode { get; set; }
        public string ApplicationCode { get; set; }
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
    }
}
