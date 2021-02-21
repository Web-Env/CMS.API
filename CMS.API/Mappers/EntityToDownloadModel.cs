using AutoMapper;
using CMS.API.DownloadModels;
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
