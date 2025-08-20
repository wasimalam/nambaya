using Common.DataAccess.Models;

namespace UserManagement.Repository.Models
{
    public class NotificationTemplate : BaseModel
    {
        #region Data Members
        private long _eventtypeid;
        private long _templatetypeid;
        private string _code;
        private string _subject;
        private string _message;
        private bool _isactive;
        #endregion

        #region Properties
        public long EventTypeID
        {
            get { return _eventtypeid; }
            set
            {
                if (_eventtypeid != value)
                {
                    _eventtypeid = value;
                    SetField(ref _eventtypeid, value, "EventTypeID");
                }
            }
        }
        public long TemplateTypeID
        {
            get { return _templatetypeid; }
            set
            {
                if (_templatetypeid != value)
                {
                    _templatetypeid = value;
                    SetField(ref _templatetypeid, value, "TemplateTypeID");
                }
            }
        }
        public string Code
        {
            get { return _code; }
            set
            {
                if (_code != value)
                {
                    _code = value;
                    SetField(ref _code, value, "Code");
                }
            }
        }
        public string Subject
        {
            get { return _subject; }
            set
            {
                if (_subject != value)
                {
                    _subject = value;
                    SetField(ref _subject, value, "Subject");
                }
            }
        }
        public string Message
        {
            get { return _message; }
            set
            {
                if (_message != value)
                {
                    _message = value;
                    SetField(ref _message, value, "Message");
                }
            }
        }
        public bool IsActive
        {
            get { return _isactive; }
            set
            {
                if (_isactive != value)
                {
                    _isactive = value;
                    SetField(ref _isactive, value, "IsActive");
                }
            }
        }
        #endregion
    }
}
