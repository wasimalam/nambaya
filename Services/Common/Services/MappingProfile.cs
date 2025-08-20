using AutoMapper;
using Common.BusinessObjects;
using Common.DataAccess.Models;

namespace Common.Services
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Lookups, LookupsBO>().ReverseMap();
            CreateMap<Language, LanguageBO>().ReverseMap();
        }
    }
}