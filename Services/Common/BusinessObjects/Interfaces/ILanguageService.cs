using System.Collections.Generic;

namespace Common.BusinessObjects.Interfaces
{
    public interface ILanguageService
    {
        IEnumerable<LanguageBO> GetLanguages();
    }
}
