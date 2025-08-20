namespace UserManagement.Contracts.Models
{
    public class NotificationEventTypeBO
    {
        public long ID { get; set; }
        public long EventTypeID { get; set; }
        public long NotificationTypeID { get; set; }
        public bool IsActive { get; set; }
        public bool LogIt { get; set; }
    }
}
