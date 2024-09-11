using SocialMedia.Post.Query.Migrations.Runner;
using Testcontainers.PostgreSql;

namespace SocialMedia.Post.Query.Tests
{
    public class DatabaseTestBase
    {
        private PostgreSqlContainer _container;

        public string ConnectionString => _container.GetConnectionString();

        [OneTimeSetUp]
        public async Task SetupOnceAsync()
        {
            _container = new PostgreSqlBuilder()
                .WithDatabase("testdb")
                .WithUsername("postgres")
                .WithPassword("password")
                .Build();

            await _container.StartAsync();

            RunMigrations();
        }

        private void RunMigrations()
        {
            var migratorSettings = new MigratorSettings()
            {
                DatabaseConnectionString = _container.GetConnectionString()
            };

            var migrator = new PostDbMigrator(migratorSettings);
            migrator.MigrateUp();
        }

        [OneTimeTearDown]
        public async Task TearDownAsync()
        {
            await _container.StopAsync();
        }


    }
}