using Common.DataAccess;
using Common.DataAccess.Models;
using System;

namespace Patient.Repository.Models
{
    public class CaseNotes : BaseModel
    {
        #region Data Members
        private long _patientcaseid;
        private string _notes;
        #endregion
        [DapperIgnore]
        public override string UpdatedBy { get => base.UpdatedBy; set => base.UpdatedBy = value; }
        [DapperIgnore]
        public override DateTime? UpdatedOn { get => base.UpdatedOn; set => base.UpdatedOn = value; }
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
        public string Notes
        {
            get => _notes;
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
