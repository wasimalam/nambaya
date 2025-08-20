using AutoMapper;
using CentralGroup.Contracts.Models;

namespace CentralGroup.Service
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Repository.Models.CentralGroup, CentralGroupBO>().ReverseMap();
        }
    }
}
