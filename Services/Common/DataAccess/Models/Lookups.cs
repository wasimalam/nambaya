namespace Common.DataAccess.Models
{
    public class Lookups : BaseLookup
    {
        #region Data Members
        private int _lookupcatid;
        private string _value;
        #endregion

        #region Properties
        public int LookupCatID
        {
            get { return _lookupcatid; }
            set
            {
                if (_lookupcatid != value)
                {
                    _lookupcatid = value;
                    SetField(ref _lookupcatid, value, "LookupCatID");
                }
            }
        }
        public string Value
        {
            get { return _value; }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    SetField(ref _value, value, "Value");
                }
            }
        }
        #endregion
    }
}
