using Common.DataAccess.Models;
using System;

namespace Patient.Repository.Models
{
    public partial class DeviceAssignment : BaseModel
    {
        #region Data Members
        private long _deviceid;
        private long _patientcaseid;
        private DateTime _assignmentDate;
        private bool _isassigned;
        #endregion

        #region Properties

        public long DeviceID
        {
            get { return _deviceid; }
            set
            {
                if (_deviceid != value)
                {
                    _deviceid = value;
                    SetField(ref _deviceid, value, "DeviceID");
                }
            }
        }
        public long PatientCaseID
        {
            get { return _patientcaseid; }
            set
            {
                if (_patientcaseid != value)
                {
                    _patientcaseid = value;
                    SetField(ref _patientcaseid, value, "PatientCaseID");
                }
            }
        }
        public DateTime AssignmentDate
        {
            get { return _assignmentDate; }
            set
            {
                if (_assignmentDate != value)
                {
                    _assignmentDate = value;
                    SetField(ref _assignmentDate, value, "AssignmentDate");
                }
            }
        }
        public bool IsAssigned
        {
            get { return _isassigned; }
            set
            {
                if (_isassigned != value)
                {
                    _isassigned = value;
                    SetField(ref _isassigned, value, "IsAssigned");
                }
            }
        }
        #endregion
    }
}
