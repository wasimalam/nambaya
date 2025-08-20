using Common.DataAccess.Models;
using System.Collections.Generic;

namespace Common.DataAccess.Interfaces
{
    public interface ILookupsRepository : IDapperRepositoryBase<Lookups>
    {
        IEnumerable<Lookups> GetItemsByCategoryCode(string categorycode);
    }
}