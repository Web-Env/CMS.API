using AutoMapper;
using CMS.API.DownloadModels.Content;
using CMS.API.DownloadModels.User;
using CMS.Domain.Entities;

namespace CMS.API.Infrastructure.Mappers
{
    public class EntityToDownloadModelProfile : Profile
    {
        public EntityToDownloadModelProfile()
        {
            CreateMap<Content, ContentDownloadModel>()
                .ForMember(dest => dest.Title, src => src.MapFrom(a => a.Title))
                .ForMember(dest => dest.Path, src => src.MapFrom(a => a.Path))
                .ForMember(dest => dest.SectionId, src => src.MapFrom(a => a.SectionId))
                .ForMember(dest => dest.Section, src => src.MapFrom(a => a.Section))
                .ForMember(dest => dest.CreatedOn, src => src.MapFrom(a => a.CreatedOn))
                .ForMember(dest => dest.CreatedBy, src => src.MapFrom(a => a.CreatedByNavigation))
                .ForMember(dest => dest.LastUpdatedOn, src => src.MapFrom(a => a.LastUpdatedOn))
                .ForMember(dest => dest.LastUpdatedBy, src => src.MapFrom(a => a.LastUpdatedBy));

            CreateMap<ContentTimeTracking, ContentTimeTrackingDownloadModel>()
                .ForMember(dest => dest.Id, src => src.MapFrom(a => a.Id))
                .ForMember(dest => dest.ContentId, src => src.MapFrom(a => a.ContentId))
                .ForMember(dest => dest.UserId, src => src.MapFrom(a => a.UserId))
                .ForMember(dest => dest.TotalTime, src => src.MapFrom(a => a.TotalTime))
                .ForMember(dest => dest.LastSeen, src => src.MapFrom(a => a.LastSeen));

            CreateMap<Section, SectionDownloadModel>()
                .ForMember(dest => dest.Title, src => src.MapFrom(a => a.Title))
                .ForMember(dest => dest.Path, src => src.MapFrom(a => a.Path))
                .ForMember(dest => dest.Contents, src => src.MapFrom(a => a.Contents))
                .ForMember(dest => dest.CreatedOn, src => src.MapFrom(a => a.CreatedOn))
                .ForMember(dest => dest.CreatedBy, src => src.MapFrom(a => a.CreatedByNavigation))
                .ForMember(dest => dest.LastUpdatedOn, src => src.MapFrom(a => a.LastUpdatedOn))
                .ForMember(dest => dest.LastUpdatedBy, src => src.MapFrom(a => a.LastUpdatedBy));

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
