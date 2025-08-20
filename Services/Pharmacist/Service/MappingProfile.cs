using AutoMapper;
using Pharmacist.Contracts.Models;
using Pharmacist.Repository.Models;

namespace Pharmacist.Business
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Pharmacy, PharmacyBO>().ReverseMap();
            CreateMap<Repository.Models.Pharmacist, PharmacistBO>().ReverseMap();
            CreateMap<PharmacyPatients, PharmacyPatientsBO>().ReverseMap();
            CreateMap<Device, DeviceBO>().ReverseMap();
            CreateMap<DeviceAssignment, DeviceAssignmentBO>().ReverseMap();
        }
    }
}
