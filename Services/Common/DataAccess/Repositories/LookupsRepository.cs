using Common.DataAccess.Interfaces;
using Common.DataAccess.Models;
using System.Collections.Generic;

namespace Common.DataAccess.Repositories
{
    public class LookupsRepository : DapperRepositoryBase<Lookups>, ILookupsRepository
    {
        public LookupsRepository(IDatabaseSession databaseSession) : base(databaseSession)
        {

        }

        public IEnumerable<Lookups> GetItemsByCategoryCode(string categorycode)
        {
            return GetItems(System.Data.CommandType.Text, $"select l.* from Lookups l inner JOIN LookupCategories lc on l.LookupCatID=lc.ID where lc.Code=@categorycode", new { categorycode = categorycode });
        }
    }
}
