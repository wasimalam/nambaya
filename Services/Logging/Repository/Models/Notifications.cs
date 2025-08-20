using Common.DataAccess;
using Common.DataAccess.Models;
using System;

namespace Logging.Repository.Models
{
    public class Notifications : BaseModel
    {
        #region Data Member
        private string _message;
        private string _subject;
        private string _recipient;
        private long _eventtypeid;
        private long _templatetypeid;
        private DateTime _timestamp;
        #endregion
        [DapperIgnore]
        public override string CreatedBy { get => base.CreatedBy; set => base.CreatedBy = value; }
        [DapperIgnore]
        public override DateTime? CreatedOn { get => base.CreatedOn; set => base.CreatedOn = value; }
        [DapperIgnore]
        public override string UpdatedBy { get => base.UpdatedBy; set => base.UpdatedBy = value; }
        [DapperIgnore]
        public override DateTime? UpdatedOn { get => base.UpdatedOn; set => base.UpdatedOn = value; }

        #region Properties
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
        public string Recipient
        {
            get { return _recipient; }
            set
            {
                if (_recipient != value)
                {
                    _recipient = value;
                    SetField(ref _recipient, value, "Recipient");
                }
            }
        }
        public long EventTypeId
        {
            get { return _eventtypeid; }
            set
            {
                if (_eventtypeid != value)
                {
                    _eventtypeid = value;
                    SetField(ref _eventtypeid, value, "EventTypeId");
                }
            }
        }
        public long TemplateTypeId
        {
            get { return _templatetypeid; }
            set
            {
                if (_templatetypeid != value)
                {
                    _templatetypeid = value;
                    SetField(ref _templatetypeid, value, "TemplateTypeId");
                }
            }
        }
        public DateTime TimeStamp
        {
            get { return _timestamp; }
            set
            {
                if (_timestamp != value)
                {
                    _timestamp = value;
                    SetField(ref _timestamp, value, "TimeStamp");
                }
            }
        }
        #endregion
    }
}
