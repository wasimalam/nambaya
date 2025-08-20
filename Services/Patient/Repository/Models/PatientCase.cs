using Common.DataAccess.Models;
using System;

namespace Patient.Repository.Models
{
    public partial class PatientCases : BaseModel
    {
        #region Data Members
        private long _patientid;
        private DateTime? _startdate;
        private DateTime? _enddate;
        private bool _isactive;
        private long _statusid;
        private long _stepid;
        private long? _cardiologistid;
        private long? _doctorid;
        #endregion

        #region Properties
        public long PatientID
        {
            get { return _patientid; }
            set
            {
                if (_patientid != value)
                {
                    _patientid = value;
                    SetField(ref _patientid, value, "PatientID");
                }
            }
        }
        public DateTime? StartDate
        {
            get { return _startdate; }
            set
            {
                if (_startdate != value)
                {
                    _startdate = value;
                    SetField(ref _startdate, value, "StartDate");
                }
            }
        }
        public DateTime? EndDate
        {
            get { return _enddate; }
            set
            {
                if (_enddate != value)
                {
                    _enddate = value;
                    SetField(ref _enddate, value, "EndDate");
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
        public long StatusID
        {
            get { return _statusid; }
            set
            {
                if (_statusid != value)
                {
                    _statusid = value;
                    SetField(ref _statusid, value, "StatusID");
                }
            }
        }
        public long StepID
        {
            get { return _stepid; }
            set
            {
                if (_stepid != value)
                {
                    _stepid = value;
                    SetField(ref _stepid, value, "StepID");
                }
            }
        }
        public long? CardiologistID
        {
            get { return _cardiologistid; }
            set
            {
                if (_cardiologistid != value)
                {
                    _cardiologistid = value;
                    SetField(ref _cardiologistid, value, "CardiologistID");
                }
            }
        }
        public long? DoctorID
        {
            get { return _doctorid; }
            set
            {
                if (_doctorid != value)
                {
                    _doctorid = value;
                    SetField(ref _doctorid, value, "DoctorID");
                }
            }
        }
        #endregion
    }
}
