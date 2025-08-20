using Common.DataAccess;
using Common.DataAccess.Models;
using System.Collections.Generic;

namespace Patient.Repository.Models
{
    public partial class DrugGroup : BaseModel
    {
        #region Data Members
        private long _patientcaseid;
        private long? _druggroupcodeid;
        private string _druggroupname;
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
        public long? DrugGroupCodeID
        {
            get => _druggroupcodeid;
            set
            {
                if (_druggroupcodeid != value)
                {
                    _druggroupcodeid = value;
                    SetField(ref _druggroupcodeid, value, "DrugGroupCodeID");
                }
            }
        }
        public string DrugGroupName
        {
            get { return _druggroupname; }
            set
            {
                if (_druggroupname != value)
                {
                    _druggroupname = value;
                    SetField(ref _druggroupname, value, "DrugGroupName");
                }
            }
        }
        #endregion
        [DapperIgnore]
        private List<DrugDetails> DrugDetailsList { get; set; } = new List<DrugDetails>();
        [DapperIgnore]
        private List<DrugFreeText> DrugFreeTextList { get; set; } = new List<DrugFreeText>();
        [DapperIgnore]
        private List<DrugReceipe> DrugReceipeList { get; set; } = new List<DrugReceipe>();

    }
}
