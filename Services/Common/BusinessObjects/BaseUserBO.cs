namespace Common.BusinessObjects
{
    public class BaseUserBO : BaseBO
    {
        public string ApplicationCode { get; set; }
        public string LoginName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
        public bool IsLocked { get; set; }
        public bool IsPasswordResetRequired { get; set; }
    }
}