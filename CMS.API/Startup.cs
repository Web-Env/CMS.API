using CMS.API.Infrastructure.Extensions;
using CMS.API.Infrastructure.Settings;
using CMS.API.Services.Authentication;
using CMS.Domain.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using WebEnv.Util.Mailer.Settings;

namespace CMS.API
{
    public class Startup
    {
        private readonly string _corsPolicy = "CorsPolicy";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                );

            services.AddCors(options =>
            {
                options.AddPolicy(_corsPolicy, builder => builder
                    .WithOrigins("http://localhost:4200")
                    .WithOrigins("https://localhost:4200")
                    .WithOrigins("https://webenv-cms.web.app")
                    .WithOrigins("https://www.webenv-cms.web.app")
                    .AllowCredentials()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed((_) => true));
            });

            services.AddAsymmetricAuthentication();

            services.AddTransient<AuthenticationService>();
            services.AddDbContext<CMSContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("CMSDb")),
                ServiceLifetime.Scoped
            );
            services.AddCustomMappers();

            var smtpSettingsSection = Configuration.GetSection("SmtpSettings");
            var emailSettingsSection = Configuration.GetSection("EmailSettings");
            var organisationSettingsSection = Configuration.GetSection("OrganisationSettings");
            var azureStorageSettings = Configuration.GetSection("AzureStorageSettings");
            services.Configure<SmtpSettings>(smtpSettingsSection);
            services.Configure<EmailSettings>(emailSettingsSection);
            services.Configure<OrganisationSettings>(organisationSettingsSection);
            services.Configure<AzureStorageSettings>(azureStorageSettings);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CMS", Version = "V0.0.1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. 
                        Enter 'Bearer' [space] and then your token in the text input below.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(_corsPolicy);

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                    ForwardedHeaders.XForwardedProto
            });

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseWebSockets();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                string swaggerJsonBasePath = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..";
                c.SwaggerEndpoint($"{swaggerJsonBasePath}/swagger/v1/swagger.json", "CMS");
            });
        }

        private CMSContext ConfigureRepositoryContext()
        {
            var options = new DbContextOptionsBuilder<CMSContext>()
                .UseSqlServer(Configuration.GetConnectionString("CMSDb"))
                .Options;
            CMSContext context = new CMSContext(options);

            return context;
        }
    }
}
