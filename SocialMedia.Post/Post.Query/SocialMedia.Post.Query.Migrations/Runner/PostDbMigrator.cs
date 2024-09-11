using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace SocialMedia.Post.Query.Migrations.Runner;

public class PostDbMigrator
{
    private readonly Assembly _migrationsAssembly;
    private MigratorSettings _settings;

    public PostDbMigrator(MigratorSettings settings)
    {
        _migrationsAssembly = this.GetType().Assembly;
        _settings = settings;
    }

    public void MigrateUp()
    {
        using (var services = CreateServices())
        using (var scope = services.CreateScope())
        {
            var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();
        }
    }

    private ServiceProvider CreateServices()
    {
        var services = new ServiceCollection()
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                    .AddPostgres()
                    .WithGlobalConnectionString(_settings.DatabaseConnectionString)
                    .ScanIn(_migrationsAssembly).For.Migrations())
            .AddLogging(lb => lb.AddFluentMigratorConsole());

        return services.BuildServiceProvider(false);
    }
}