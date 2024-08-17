using UraDocs.ApiService.Extensions;
using UraDocs.ApiService.Helpers;
using UraDocs.Shared;

namespace UraDocs.ApiService.Services;

[Obsolete]
public class MenuWatcherBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private FileSystemWatcher _watcher = null!;

    public MenuWatcherBackgroundService(
        IServiceProvider serviceProvider
        , IWebHostEnvironment webHost)
    {
        _serviceProvider = serviceProvider;

        PrepareWatcher().Wait();
    }

    string GetUraMenuPath() => Directory.GetCurrentDirectory();

    private async Task PrepareWatcher()
    {
        var menuPath = FileHelper.GetUraMenuPath();

        if (!File.Exists(menuPath))
        {
            var menu = new UraMenu();
            await File.WriteAllTextAsync(menuPath, menu.ToJson());
        }

        _watcher = new FileSystemWatcher()
        {
            Path = menuPath,
            Filter = Contas.UraMenuFileName,
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size
        };

        _watcher.Changed += OnChanged;
    }

    private void OnChanged(object source, FileSystemEventArgs e)
    {
        using var scope = _serviceProvider.CreateScope();
        var menuService = scope.ServiceProvider.GetRequiredService<MenuService>();

        menuService.ReloadMenus();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _watcher.EnableRaisingEvents = true;
        return Task.CompletedTask;
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _watcher.EnableRaisingEvents = false;
        return Task.CompletedTask;
    }
}
