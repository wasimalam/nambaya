using Common.DataAccess.Models;

namespace Patient.Repository.Models
{
    public partial class PatientAdditionalInfo : BaseModel
    {
        #region Data Members        
        private long _patientcaseid;
        private string _allergiesfreetext;
        private bool _ispregnant;
        private bool _isfeeding;
        private bool _isnursecase;
        private decimal _weight;
        private decimal _height;
        private decimal _creatininevalue;
        private int? _healthstatus;
        private string _additionalfreetext;

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
        public string AllergiesFreeText
        {
            get { return _allergiesfreetext; }
            set
            {
                if (_allergiesfreetext != value)
                {
                    _allergiesfreetext = value;
                    SetField(ref _allergiesfreetext, value, "AllergiesFreeText");
                }
            }
        }
        public bool IsPregnant
        {
            get { return _ispregnant; }
            set
            {
                if (_ispregnant != value)
                {
                    _ispregnant = value;
                    SetField(ref _ispregnant, value, "IsPregnant");
                }
            }
        }
        public bool IsFeeding
        {
            get { return _isfeeding; }
            set
            {
                if (_isfeeding != value)
                {
                    _isfeeding = value;
                    SetField(ref _isfeeding, value, "IsFeeding");
                }
            }
        }
        public bool IsNurseCase
        {
            get { return _isnursecase; }
            set
            {
                if (_isnursecase != value)
                {
                    _isnursecase = value;
                    SetField(ref _isnursecase, value, "IsNurseCase");
                }
            }
        }
        public decimal Weight
        {
            get { return _weight; }
            set
            {
                if (_weight != value)
                {
                    _weight = value;
                    SetField(ref _weight, value, "Weight");
                }
            }
        }
        public decimal Height
        {
            get { return _height; }
            set
            {
                if (_height != value)
                {
                    _height = value;
                    SetField(ref _height, value, "Height");
                }
            }
        }
        public decimal CreatinineValue
        {
            get { return _creatininevalue; }
            set
            {
                if (_creatininevalue != value)
                {
                    _creatininevalue = value;
                    SetField(ref _creatininevalue, value, "CreatinineValue");
                }
            }
        }
        public int? HealthStatus
        {
            get { return _healthstatus; }
            set
            {
                if (_healthstatus != value)
                {
                    _healthstatus = value;
                    SetField(ref _healthstatus, value, "HealthStatus");
                }
            }
        }
        public string AdditionalFreeText
        {
            get { return _additionalfreetext; }
            set
            {
                if (_additionalfreetext != value)
                {
                    _additionalfreetext = value;
                    SetField(ref _additionalfreetext, value, "AdditionalFreeText");
                }
            }
        }
        #endregion
    }
}
