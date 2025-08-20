namespace Common.DataAccess.Models
{
    public class BaseUser : BaseModel
    {
        public string ApplicationCode { get; set; }
        public string LoginId { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
        public bool IsLocked { get; set; }
    }
}
