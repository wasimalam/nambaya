using Common.DataAccess;
using Common.DataAccess.Models;
using System;

namespace Logging.Repository.Models
{
    public class SeriLogs : BaseModel
    {
        #region Data Member
        private string _message;
        private string _level;
        private DateTime _timestamp;
        private string _exception;
        private string _applicationname;
        private string _requestid;
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
        public string Level
        {
            get { return _level; }
            set
            {
                if (_level != value)
                {
                    _level = value;
                    SetField(ref _level, value, "Level");
                }
            }
        }
        public string Exception
        {
            get { return _exception; }
            set
            {
                if (_exception != value)
                {
                    _exception = value;
                    SetField(ref _exception, value, "Exception");
                }
            }
        }
        public string ApplicationName
        {
            get { return _applicationname; }
            set
            {
                if (_applicationname != value)
                {
                    _applicationname = value;
                    SetField(ref _applicationname, value, "ApplicationName");
                }
            }
        }
        public string CorrelationId
        {
            get { return _requestid; }
            set
            {
                if (_requestid != value)
                {
                    _requestid = value;
                    SetField(ref _requestid, value, "CorrelationId");
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
