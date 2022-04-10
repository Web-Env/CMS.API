using CMS.API.Tests;
using CMS.Domain.Entities;
using CMS.Domain.Repositories;
using CMS.Domain.Repositories.Contexts;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CMS.API.Services.Tests
{
    public class ServiceTestBase : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _databaseFixture;
        public IRepositoryManager RepositoryManager { get; private set; }

        protected ServiceTestBase(DatabaseFixture fixture)
        {
            _databaseFixture = fixture;
            RepositoryManager = new RepositoryManager(CreateTestRepositoryContext());
        }

        public CMSContext CreateTestRepositoryContext()
        {
            var options = new DbContextOptionsBuilder<CMSContext>()
                .UseSqlite(_databaseFixture.GetConnection())
                .Options;
            CMSContext context = new CMSContext(options);

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
