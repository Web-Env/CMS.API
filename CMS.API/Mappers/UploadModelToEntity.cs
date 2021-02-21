using AutoMapper;
using CMS.API.Mappers.Converters;
using CMS.API.UploadModels;
using CMS.Domain.Entities;

namespace CMS.API.Mappers
{
    public class UploadModelToEntity : Profile
    {
        public UploadModelToEntity()
        {
            CreateMap<UserUploadModel, User>()
                .ForMember(dest => dest.Password, src => src.ConvertUsing(new HashConverter()));
        }
    }
}
