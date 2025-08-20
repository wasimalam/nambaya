using AutoMapper;
using Logging.Contracts.Models;

namespace Logging.Service
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Repository.Models.SeriLogs, SeriLogsBO>().ReverseMap();
            CreateMap<Repository.Models.Notifications, NotificationsBO>().ReverseMap();
        }
    }
}
