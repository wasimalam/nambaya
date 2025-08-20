using Common.BusinessObjects;

namespace UserManagement.Contracts.Models
{
    public class UserSettingBO1 : BaseBO
    {
        public long UserId { get; set; }
        public string Code { get; set; }
        public long ApplicationID { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public string DataType { get; set; }
    }
}