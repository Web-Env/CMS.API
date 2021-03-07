using AutoMapper;
using CMS.API.DownloadModels.User;
using CMS.Domain.Entities;

namespace CMS.API.Mappers
{
    public class EntityToDownloadModel : Profile
    {
        public EntityToDownloadModel()
        {
            CreateMap<User, UserDownloadModel>();
        }
    }
}
