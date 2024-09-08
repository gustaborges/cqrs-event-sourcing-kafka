using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SocialMedia.Post.Query.Migrations;

internal class Program
{
    static void Main(string[] args)
    {
        using (var serviceProvider = CreateServices())
        using (var scope = serviceProvider.CreateScope())
        {
            UpdateDatabase(scope.ServiceProvider);
        }
    }

    private static ServiceProvider CreateServices()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        var services = new ServiceCollection()
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                    .AddPostgres()
                    .WithGlobalConnectionString(connectionString)
                    .ScanIn(typeof(Program).Assembly).For.Migrations())
            .AddLogging(lb => lb.AddFluentMigratorConsole());

        return services.BuildServiceProvider(false);
    }

    private static void UpdateDatabase(IServiceProvider serviceProvider)
    {
        var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }
}
