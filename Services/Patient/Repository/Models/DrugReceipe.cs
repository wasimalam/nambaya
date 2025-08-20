using Common.DataAccess.Models;

namespace Patient.Repository.Models
{
    public partial class DrugReceipe : BaseModel
    {
        #region Data Members
        private long _druggroupid;
        private string _formulationtext;
        private string _formulationadditionaltext;
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
        public string FormulationText
        {
            get => _formulationtext;
            set
            {
                if (_formulationtext != value)
                {
                    _formulationtext = value;
                    SetField(ref _formulationtext, value, "FormulationText");
                }
            }
        }
        public string FormulationAdditionalText
        {
            get => _formulationadditionaltext;
            set
            {
                if (_formulationadditionaltext != value)
                {
                    _formulationadditionaltext = value;
                    SetField(ref _formulationadditionaltext, value, "FormulationAdditionalText");
                }
            }
        }
        #endregion
    }
}
