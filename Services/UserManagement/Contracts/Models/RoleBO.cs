using Common.BusinessObjects;

namespace UserManagement.Contracts.Models
{
    public class RoleBO : BaseBO
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public long ApplicationID { get; set; }
        public string ADGroup { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}