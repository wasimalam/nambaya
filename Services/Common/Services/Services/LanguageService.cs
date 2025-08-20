using AutoMapper;
using Common.BusinessObjects;
using Common.BusinessObjects.Interfaces;
using Common.DataAccess.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Common.Services.Services
{
    public class LanguageService : ILanguageService
    {
        private readonly ILanguageRepository _languageRepository;
        private readonly IMapper _mapper;

        public LanguageService(ILanguageRepository lookupRepository, IMapper mapper)
        {
            _languageRepository = lookupRepository;
            _mapper = mapper;
        }

        public IEnumerable<LanguageBO> GetLanguages()
        {
            var items = _languageRepository.GetLanguages();
            return items.Select(p => _mapper.Map<LanguageBO>(p)).ToList();
        }
    }
}
