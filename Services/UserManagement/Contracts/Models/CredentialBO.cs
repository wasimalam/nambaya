using Common.BusinessObjects;

namespace UserManagement.Contracts.Models
{
    public class CredentialBO : BaseBO
    {
        public long UserID { get; set; }
        public string Password { get; set; }
        public bool IsDeleted { get; set; }
    }
}