using Common.DataAccess.Models;

namespace Patient.Repository.Models
{
    public partial class QuickEvaluationResult : BaseModel
    {
        #region Data Members
        private int _patientcaseid;
        private string _measurementtime;
        private long _quickresult;
        private string _notes;
        #endregion

        #region Properties
        public int PatientCaseID
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
        public string MeasurementTime
        {
            get { return _measurementtime; }
            set
            {
                if (_measurementtime != value)
                {
                    _measurementtime = value;
                    SetField(ref _measurementtime, value, "MeasurementTime");
                }
            }
        }
        public long QuickResultID
        {
            get { return _quickresult; }
            set
            {
                if (_quickresult != value)
                {
                    _quickresult = value;
                    SetField(ref _quickresult, value, "QuickResultID");
                }
            }
        }
     
        public string Notes
        {
            get { return _notes; }
            set
            {
                if (_notes != value)
                {
                    _notes = value;
                    SetField(ref _notes, value, "Notes");
                }
            }
        }
        #endregion
    }
}
