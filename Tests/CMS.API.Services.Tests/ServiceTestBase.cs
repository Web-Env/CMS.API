using CMS.API.Tests;
using CMS.Domain.Entities;
using CMS.Domain.Repositories;
using CMS.Domain.Repositories.Contexts;
using CMS.Domain.Repositories.Interfaces;
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
