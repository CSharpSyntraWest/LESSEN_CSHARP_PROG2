using Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyEmployees.MVC.Test
{
    public class TestRepositoryContextFactory : IDisposable
    {
        private DbConnection _connection;

        private DbContextOptions<RepositoryContext> CreateOptions()
        {
            return new DbContextOptionsBuilder<RepositoryContext>()
                .UseSqlite(_connection).Options;
        }

        public RepositoryContext CreateContext()
        {
            if (_connection == null)
            {
                _connection = new SqliteConnection("DataSource=:memory:");
                _connection.Open();

                var options = CreateOptions();
                using (var context = new RepositoryContext(options))
                {
                    context.Database.EnsureCreated();
                }
            }

            return new RepositoryContext(CreateOptions());
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }
    }

}
