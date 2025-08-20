namespace Common.DataAccess.Models
{
    public class LookupsTranslation : BaseLookup
    {
        #region Data Members
        private int _lookupid;
        private int _languageid;
        private int _value;
        #endregion

        #region Properties
        public int LookupID
        {
            get { return _lookupid; }
            set
            {
                if (_lookupid != value)
                {
                    _lookupid = value;
                    SetField(ref _lookupid, value, "LookupID");
                }
            }
        }
        public int LanguageID
        {
            get { return _languageid; }
            set
            {
                if (_languageid != value)
                {
                    _languageid = value;
                    SetField(ref _languageid, value, "LanguageID");
                }
            }
        }
        public int Value
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
