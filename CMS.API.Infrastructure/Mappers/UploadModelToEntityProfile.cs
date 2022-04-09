using AutoMapper;
using CMS.API.UploadModels.User;
using CMS.Domain.Entities;

namespace CMS.API.Infrastructure.Mappers
{
    public class UploadModelToEntityProfile : Profile
    {
        public UploadModelToEntityProfile()
        {
            CreateMap<UserUploadModel, User>();
        }
    }
}
