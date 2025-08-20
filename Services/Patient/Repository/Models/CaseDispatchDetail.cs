using Common.DataAccess.Models;
using System;

namespace Patient.Repository.Models
{
    public partial class CaseDispatchDetail : BaseModel
    {
        #region Data Members
        private long _patientcaseid;
        private DateTime _dispatchdate;
        private bool _ismedicationplanattached;
        private bool _isdetailevaluationattached;
        #endregion

        #region Properties
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
        public DateTime DispatchDate
        {
            get => _dispatchdate;
            set
            {
                if (_dispatchdate != value)
                {
                    _dispatchdate = value;
                    SetField(ref _dispatchdate, value, "DispatchDate");
                }
            }
        }
        public bool IsMedicationPlanAttached
        {
            get => _ismedicationplanattached;
            set
            {
                if (_ismedicationplanattached != value)
                {
                    _ismedicationplanattached = value;
                    SetField(ref _ismedicationplanattached, value, "IsMedicationPlanAttached");
                }
            }
        }
        public bool IsDetailEvaluationAttached
        {
            get => _isdetailevaluationattached;
            set
            {
                if (_isdetailevaluationattached != value)
                {
                    _isdetailevaluationattached = value;
                    SetField(ref _isdetailevaluationattached, value, "IsDetailEvaluationAttached");
                }
            }
        }
        #endregion
    }
}
