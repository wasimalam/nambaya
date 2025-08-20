using System.Collections.Generic;

namespace Common.DataAccess.Interfaces
{
    public interface IPropertyContainer
    {

        #region Properties

        IEnumerable<string> IdNames
        {
            get;
        }

        IEnumerable<string> ValueNames
        {
            get;
        }

        IEnumerable<string> AllNames
        {
            get;
        }

        IDictionary<string, object> IdPairs
        {
            get;
        }

        IDictionary<string, object> ValuePairs
        {
            get;
        }

        IEnumerable<KeyValuePair<string, object>> AllPairs
        {
            get;
        }

        #endregion

        #region Constructor


        #endregion

        #region Methods

        void AddId(string name, object value);


        void AddValue(string name, object value);

        #endregion
    }
}
