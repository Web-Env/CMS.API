using CMS.Domain.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;

namespace CMS.API.Tests
{
    public class DatabaseFixture : IDisposable
    {
        private CMSContext _context;

        private const string _connectionString = "DataSource=:memory:";
        private SqliteConnection _connection;

        public DatabaseFixture()
        {
            _connection = new SqliteConnection(_connectionString);
            _connection.Open();

            var options = new DbContextOptionsBuilder<CMSContext>()
                .UseSqlite(_connection)
                .Options;

            _context = new CMSContext(options);
            _context.Database.EnsureCreated();
        }

        public CMSContext GetContext()
        {
            return _context;
        }

        public CMSContext NewContext()
        {
            _connection = new SqliteConnection(_connectionString);
            _connection.Open();

            var options = new DbContextOptionsBuilder<CMSContext>()
                .UseSqlite(_connection)
                .Options;

            _context = new CMSContext(options);
            _context.Database.EnsureCreated();

            return GetContext();
        }

        public SqliteConnection GetConnection()
        {
            return _connection;
        }

        public void Dispose()
        {
            _connection.Close();
        }
    }
}
