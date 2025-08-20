using Common.DataAccess.Interfaces;
using Common.DataAccess.Models;
using System.Collections.Generic;

namespace Common.DataAccess.Repositories
{
    public class LanguageRepository : DapperRepositoryBase<Language>, ILanguageRepository
    {
        public LanguageRepository(IDatabaseSession databaseSession) : base(databaseSession)
        {

        }

        public IEnumerable<Language> GetLanguages()
        {
            return SelectAll();
        }
    }
}
