using AutoMapper;
using Common.BusinessObjects;
using Common.DataAccess.Models;
using UserManagement.Contracts.Models;

namespace UserManagement.Service
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<BaseUser, BaseUserBO>().ReverseMap();
            CreateMap<Repository.Models.Credential, CredentialBO>().ReverseMap();
            CreateMap<Repository.Models.Role, RoleBO>().ReverseMap();
            CreateMap<Repository.Models.User, UserBO>().ReverseMap();
            CreateMap<Repository.Models.UserSetting, UserSettingBO>().ReverseMap();
            CreateMap<Repository.Models.User, SessionContext>().ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.ID));
            CreateMap<Repository.Models.NotificationEventParam, NotificationEventParamBO>().ReverseMap();
            CreateMap<Repository.Models.NotificationEventType, NotificationEventTypeBO>().ReverseMap();
            CreateMap<Repository.Models.NotificationTemplate, NotificationTemplateBO>().ReverseMap();
        }
    }
}
