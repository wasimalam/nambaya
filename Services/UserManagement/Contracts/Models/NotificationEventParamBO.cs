namespace UserManagement.Contracts.Models
{
    public class NotificationEventParamBO
    {
        public long ID { get; set; }
        public long EventTypeID { get; set; }
        public string Code { get; set; }
    }
}
