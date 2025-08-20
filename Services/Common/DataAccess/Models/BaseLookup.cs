namespace Common.DataAccess.Models
{
    public class BaseLookup : BaseModel
    {
        #region Data Members
        private string _code;
        private string _description;
        #endregion

        #region Properties

        public virtual string Code
        {
            get { return _code; }
            set
            {
                if (_code != value)
                {
                    _code = value;
                    SetField(ref _code, value, "Code");
                }
            }
        }


        public virtual string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    SetField(ref _description, value, "Description");
                }
            }
        }
        #endregion
    }
}
