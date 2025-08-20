using AutoMapper;
using Common.BusinessObjects;
using Common.BusinessObjects.Interfaces;
using Common.DataAccess.Interfaces;
using Common.DataAccess.Models;
using Common.Infrastructure;
using Common.Infrastructure.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Services.Services
{
    public class LookupService : BaseService, ILookupService
    {
        private readonly ILookupsRepository _lookupRepository;
        private readonly IDapperRepositoryBase<LookupCategories> _lookupCategoryRepository;
        private readonly IMapper _mapper;
        public LookupService(IServiceProvider serviceProvider, IMapper mapper) : base(serviceProvider)
        {
            _lookupCategoryRepository = serviceProvider.GetRequiredService<IDapperRepositoryBase<LookupCategories>>();
            _lookupRepository = serviceProvider.GetRequiredService<ILookupsRepository>();
            _mapper = mapper;
        }

        public IEnumerable<LookupsBO> GetItems(string categorycode)
        {
            var items = _lookupRepository.GetItemsByCategoryCode(categorycode);
            return items.Select(
                p => _mapper.Map<LookupsBO>(p)).ToList();
        }
        public long Insert(LookupsBO lookupsBO)
        {
            string categorycode = _lookupCategoryRepository.GetByID(lookupsBO.LookupCatID).Code;
            var items = _lookupRepository.GetItemsByCategoryCode(categorycode);
            var newLookupItem = _mapper.Map<DataAccess.Models.Lookups>(lookupsBO);
            long maxid = items.OrderByDescending(item => item.ID).First().ID;

            if (LookUpCategoryCodes.LookUpCategories[categorycode].Custom == 0)
                throw new ServiceException(ClientSideErrors.INVALID_USER_ACTION);
            SessionContext sessionContext = GetSessionContext();
            lookupsBO.CreatedBy = sessionContext?.LoginName ?? SystemUsers.System;
            if (maxid >= LookUpCategoryCodes.LookUpCategories[categorycode].Custom)
                newLookupItem.ID = maxid + 1;
            else
                newLookupItem.ID = LookUpCategoryCodes.LookUpCategories[categorycode].Custom;
            newLookupItem.Value = newLookupItem.ID.ToString();
            _lookupRepository.Insert(newLookupItem);
            return newLookupItem.ID;
        }
        public LookupsBO Get(long id)
        {
            return _mapper.Map<LookupsBO>(_lookupRepository.GetByID(id));
        }
    }
}
