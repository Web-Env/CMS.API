using AutoMapper;
using CMS.API.Infrastructure.Settings;
using CMS.API.Mappers;
using CMS.API.Tests.Funcs;
using CMS.API.Tests.Helpers;
using CMS.Domain.Entities;
using CMS.Domain.Repositories;
using CMS.Domain.Repositories.Contexts;
using Microsoft.EntityFrameworkCore;
using Nito.AsyncEx;
using WebEnv.Util.Mailer.Settings;
using Xunit;

namespace CMS.API.Tests.ControllerTests
{
    public class ControllerTestBase : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _databaseFixture;
        public IRepositoryManager RepositoryManager { get; private set; }
        public IMapper Mapper { get; private set; }

        public SmtpSettings SmtpSettings { get; private set; }
        public OrganisationSettings OrganisationSettings { get; private set; }

        protected ControllerTestBase(DatabaseFixture fixture)
        {
            _databaseFixture = fixture;
            RepositoryManager = new RepositoryManager(CreateTestRepositoryContext());
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new UploadModelToEntity());
                cfg.AddProfile(new EntityToDownloadModel());
            });
            Mapper = mockMapper.CreateMapper();
            SmtpSettings = EmailServiceHelper.GetSmtpSettings();
            OrganisationSettings = EmailServiceHelper.GetOrganisationSettings();

            AsyncContext.Run(() => UserFunc.CreateRootUser(GetContext()));
        }

        public CMSRepositoryContext CreateTestRepositoryContext()
        {
            var options = new DbContextOptionsBuilder<CMSRepositoryContext>()
                .UseSqlite(_databaseFixture.GetConnection())
                .Options;
            CMSRepositoryContext context = new CMSRepositoryContext(options);

            return context;
        }

        public CMSContext GetContext()
        {
            return _databaseFixture.GetContext();
        }

        public CMSContext NewContext()
        {
            var context = _databaseFixture.NewContext();
            RepositoryManager = new RepositoryManager(CreateTestRepositoryContext());
            return context;
        }
    }
}
