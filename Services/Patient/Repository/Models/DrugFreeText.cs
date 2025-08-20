using Common.DataAccess.Models;

namespace Patient.Repository.Models
{
    public class DrugFreeText : BaseModel
    {
        #region Data Members
        private long _druggroupid;
        private string _freetextInput;
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
        public string FreeTextInput
        {
            get => _freetextInput;
            set
            {
                if (_freetextInput != value)
                {
                    _freetextInput = value;
                    SetField(ref _freetextInput, value, "FreeTextInput");
                }
            }
        }
        #endregion
    }
}
