using System.Collections.Generic;

namespace Common.BusinessObjects.Interfaces
{
    public interface ILookupService
    {
        IEnumerable<LookupsBO> GetItems(string categorycode);
        long Insert(LookupsBO lookupsBO);
        LookupsBO Get(long id);
    }
}
