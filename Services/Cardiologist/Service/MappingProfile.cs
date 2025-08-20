using AutoMapper;
using Cardiologist.Contracts.Models;

namespace Cardiologist.Service
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Repository.Models.Cardiologist, CardiologistBO>().ReverseMap();
            CreateMap<Repository.Models.Nurse, NurseBO>().ReverseMap();
            CreateMap<Repository.Models.Signatures, SignaturesBO>().ReverseMap();
        }
    }
}
