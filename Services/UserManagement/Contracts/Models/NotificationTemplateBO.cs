using Common.BusinessObjects;

namespace UserManagement.Contracts.Models
{
    public class NotificationTemplateBO : BaseBO
    {
        public string Code { get; set; }
        public long TemplateTypeID { get; set; }
        public long EventTypeID { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public bool IsActive { get; set; }
        public NotificationTemplateBO()
        {
        }
        public NotificationTemplateBO(NotificationTemplateBO other)
        {
            Code = other.Code;
            TemplateTypeID = other.TemplateTypeID;
            EventTypeID = other.EventTypeID;
            Subject = other.Subject;
            Message = other.Message;
            IsActive = other.IsActive;
        }
    }
}
