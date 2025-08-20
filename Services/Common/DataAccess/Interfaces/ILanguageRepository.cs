using Common.DataAccess.Models;
using System.Collections.Generic;

namespace Common.DataAccess.Interfaces
{
    public interface ILanguageRepository : IDapperRepositoryBase<Language>
    {
        IEnumerable<Language> GetLanguages();
    }
}
