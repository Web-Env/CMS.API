using CMS.Domain.Repositories;
using CMS.Domain.Repositories.Contexts;
using CMS.Domain.Repositories.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CMS.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSingleton(IRepositoryManager, new RepositoryManager(ConfigureRepositoryContext()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private CMSRepositoryContext ConfigureRepositoryContext()
        {
            var options = new DbContextOptionsBuilder<CMSRepositoryContext>()
                .UseSqlServer("Server=localhost;Database=CMS;Trusted_Connection=True;")
                .Options;
            CMSRepositoryContext context = new CMSRepositoryContext(options);

            return context;
        }
    }
}
