using Common.BusinessObjects;
using System;

namespace UserManagement.Contracts.Models
{
    public class UserBO : BaseBO
    {
        public string LoginName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDomainUser { get; set; }
        public bool IsPasswordResetRequired { get; set; }
        public bool IsLocked { get; set; }
        public int PasswordAttempts { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? LastLoggedInOn { get; set; }
        public int DaysToExpire { get; set; }
        public string Role { get; set; }
    }
}