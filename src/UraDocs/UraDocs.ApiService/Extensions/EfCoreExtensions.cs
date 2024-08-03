using UraDocs.ApiService.Data;

namespace UraDocs.ApiService.Extensions;

public static class EfCoreExtensions
{
    public class EfCoreOptions
    {
        public bool Reset { get; set; } = false;
    }

    public static void UseSqlite(this WebApplication app, Action<EfCoreOptions> action)
    {
        var options = new EfCoreOptions();
        action(options);

        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<UraDbContext>();

        try
        {
            if (options.Reset)
            {
                db.Database.EnsureDeleted();
            }

            db.Database.EnsureCreated();
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
