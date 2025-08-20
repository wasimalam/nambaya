using Common.DataAccess.Models;

namespace Patient.Repository.Models
{
    public partial class DrugDetails : BaseModel
    {
        #region Data Members
        private long _druggroupid;
        private long _pzn;
        private string _medicinename;
        private string _dosageformcode;
        private string _dosageform;
        private string _dosagemorning;
        private string _dosagenoon;
        private string _dosageevening;
        private string _dosagenight;
        private string _dosageopenscheme;
        private string _dosageunitcode;
        private string _dosageunittext;
        private string _hints;
        private string _treatmentreason;
        private string _additionaltext;
        private bool _isactive;
        #endregion

        #region Properties
        public long DrugGroupID
        {
            get { return _druggroupid; }
            set
            {
                if (_druggroupid != value)
                {
                    _druggroupid = value;
                    SetField(ref _druggroupid, value, "DrugGroupID");
                }
            }
        }
        public long PZN
        {
            get => _pzn;
            set
            {
                if (_pzn != value)
                {
                    _pzn = value;
                    SetField(ref _pzn, value, "PZN");
                }
            }
        }
        public string MedicineName
        {
            get { return _medicinename; }
            set
            {
                if (_medicinename != value)
                {
                    _medicinename = value;
                    SetField(ref _medicinename, value, "MedicineName");
                }
            }
        }
        public string DosageFormCode
        {
            get => _dosageformcode;
            set
            {
                if (_dosageformcode != value)
                {
                    _dosageformcode = value;
                    SetField(ref _dosageformcode, value, "Dosageformcode");
                }
            }
        }
        public string DosageForm
        {
            get => _dosageform;
            set
            {
                if (_dosageform != value)
                {
                    _dosageform = value;
                    SetField(ref _dosageform, value, "Dosageform");
                }
            }
        }

        public string DosageMorning
        {
            get => _dosagemorning;
            set
            {
                if (_dosagemorning != value)
                {
                    _dosagemorning = value;
                    SetField(ref _dosagemorning, value, "Dosagemorning");
                }
            }
        }

        public string DosageNoon
        {
            get => _dosagenoon;
            set
            {
                if (_dosagenoon != value)
                {
                    _dosagenoon = value;
                    SetField(ref _dosagenoon, value, "Dosagenoon");
                }
            }
        }

        public string DosageEvening
        {
            get => _dosageevening;
            set
            {
                if (_dosageevening != value)
                {
                    _dosageevening = value;
                    SetField(ref _dosageevening, value, "Dosageevening");
                }
            }
        }

        public string DosageNight
        {
            get => _dosagenight;
            set
            {
                if (_dosagenight != value)
                {
                    _dosagenight = value;
                    SetField(ref _dosagenight, value, "Dosagenight");
                }
            }
        }

        public string DosageopenScheme
        {
            get => _dosageopenscheme;
            set
            {
                if (_dosageopenscheme != value)
                {
                    _dosageopenscheme = value;
                    SetField(ref _dosageopenscheme, value, "Dosageopenscheme");
                }
            }
        }
        public string DosageUnitCode
        {
            get => _dosageunitcode;
            set
            {
                if (_dosageunitcode != value)
                {
                    _dosageunitcode = value;
                    SetField(ref _dosageunitcode, value, "Dosageunitcode");
                }
            }
        }
        public string DosageUnitText
        {
            get => _dosageunittext;
            set
            {
                if (_dosageunittext != value)
                {
                    _dosageunittext = value;
                    SetField(ref _dosageunittext, value, "Dosageunittext");
                }
            }
        }
        public string Hints
        {
            get => _hints;
            set
            {
                if (_hints != value)
                {
                    _hints = value;
                    SetField(ref _hints, value, "Hints");
                }
            }
        }
        public string TreatmentReason
        {
            get => _treatmentreason;
            set
            {
                if (_treatmentreason != value)
                {
                    _treatmentreason = value;
                    SetField(ref _treatmentreason, value, "TreatmentReason");
                }
            }
        }
        public string AdditionalText
        {
            get => _additionaltext;
            set
            {
                if (_additionaltext != value)
                {
                    _additionaltext = value;
                    SetField(ref _additionaltext, value, "Additionaltext");
                }
            }
        }
        public bool IsActive
        {
            get => _isactive;
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
