using AutoMapper;
using NambayaUser.Contracts.Models;

namespace NambayaUser.Service
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Repository.Models.User, UserBO>().ReverseMap();
            CreateMap<Repository.Models.User, BasicUser>().ReverseMap();
            CreateMap<UserBO, BasicUser>().ReverseMap();
        }
    }
}
