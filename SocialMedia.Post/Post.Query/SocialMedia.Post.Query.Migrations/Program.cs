using SocialMedia.Post.Query.Migrations.Runner;
using System.Text.Json;

namespace SocialMedia.Post.Query.Migrations;

internal class Program
{
    static void Main(string[] args)
    {
        var settings = GetMigratorSettings();
        var runner = new PostDbMigrator(settings);

        runner.MigrateUp();
        Console.WriteLine("Finish!");
        Console.ReadLine();
    }

    private static MigratorSettings GetMigratorSettings()
    {
        var jsonSettings = File.ReadAllText("migratorsettings.json");
        return JsonSerializer.Deserialize<MigratorSettings>(jsonSettings);
    }
}
