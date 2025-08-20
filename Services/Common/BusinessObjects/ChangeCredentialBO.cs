namespace Common.BusinessObjects
{
    public class ChangeCredentialBO
    {
        public long UserId { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
