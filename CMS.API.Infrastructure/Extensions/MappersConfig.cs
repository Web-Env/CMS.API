using AutoMapper;
using CMS.API.Infrastructure.Mappers;
using Microsoft.Extensions.DependencyInjection;

namespace CMS.API.Infrastructure.Extensions
{
    public static class MappersConfig
    {
        public static void AddCustomMappers(this IServiceCollection services)
        {
            var mappersConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new EntityToDownloadModelProfile());
                cfg.AddProfile(new UploadModelToEntityProfile());
            });

            var mapper = mappersConfig.CreateMapper();

            services.AddSingleton(mapper);
        }
    }
}
