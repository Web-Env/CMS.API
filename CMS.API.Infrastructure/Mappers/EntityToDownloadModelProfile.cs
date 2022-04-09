using AutoMapper;
using CMS.API.DownloadModels.User;
using CMS.Domain.Entities;

namespace CMS.API.Infrastructure.Mappers
{
    public class EntityToDownloadModelProfile : Profile
    {
        public EntityToDownloadModelProfile()
        {
            CreateMap<User, UserDownloadModel>()
                .ForMember(dest => dest.Id, src => src.MapFrom(a => a.Id))
                .ForMember(dest => dest.FirstName, src => src.MapFrom(a => a.FirstName))
                .ForMember(dest => dest.LastName, src => src.MapFrom(a => a.LastName))
                .ForMember(dest => dest.Email, src => src.MapFrom(a => a.Email))
                .ForMember(dest => dest.IsAdmin, src => src.MapFrom(a => a.IsAdmin))
                .ForMember(dest => dest.CreatedOn, src => src.MapFrom(a => a.CreatedOn))
                .ForMember(dest => dest.CreatedBy, src => src.MapFrom(a => a.CreatedBy))
                .ForMember(dest => dest.LastUpdatedOn, src => src.MapFrom(a => a.LastUpdatedOn))
                .ForMember(dest => dest.LastUpdatedBy, src => src.MapFrom(a => a.LastUpdatedBy))
                .ForAllOtherMembers(src => src.Ignore());
        }
    }
}
