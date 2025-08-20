using Common.DataAccess.Models;

namespace Patient.Repository.Models
{
    public partial class DrugIngredients : BaseModel
    {
        #region Data Members
        private long _drugdetailsid;
        private string _activeingredients;
        private string _strength;
        #endregion

        #region Properties
        public long DrugDetailsID
        {
            get { return _drugdetailsid; }
            set
            {
                if (_drugdetailsid != value)
                {
                    _drugdetailsid = value;
                    SetField(ref _drugdetailsid, value, "DrugDetailsID");
                }
            }
        }
        public string ActiveIngredients
        {
            get => _activeingredients;
            set
            {
                if (_activeingredients != value)
                {
                    _activeingredients = value;
                    SetField(ref _activeingredients, value, "ActiveIngredients");
                }
            }
        }
        public string Strength
        {
            get { return _strength; }
            set
            {
                if (_strength != value)
                {
                    _strength = value;
                    SetField(ref _strength, value, "Strength");
                }
            }
        }
        #endregion
    }
}
