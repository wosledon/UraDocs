using Microsoft.Extensions.FileProviders;
using UraDocs.ApiService.Helpers;
using UraDocs.Shared;

namespace UraDocs.ApiService.Services;

public class FileWaterBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private FileSystemWatcher _watcher = null!;

    public FileWaterBackgroundService(
        IServiceProvider serviceProvider
        , IWebHostEnvironment webHost)
    {
        _serviceProvider = serviceProvider;

        PrepareWatcher();
    }

    private void PrepareWatcher()
    {
        var markdownPath = GetMarkdownPath();
        if(!Directory.Exists(markdownPath))
        {
            Directory.CreateDirectory(markdownPath);
        }

        _watcher = new FileSystemWatcher()
        {
            Path = GetMarkdownPath(),
            Filter = "*.md",
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size
        };

        _watcher.Changed += async (s, e)=> await OnChanged(s, e);
        _watcher.Created += async (s, e) => await OnChanged(s, e);
        _watcher.Deleted += async (s, e) => await OnChanged(s, e);
        _watcher.Renamed += async (s, e) => await OnRenamed(s, e);
    }

    private string GetMarkdownPath()
    {
        return Path.Combine(Directory.GetCurrentDirectory(), Contas.MarkdownPath);
    }

    private async Task OnChanged(object source, FileSystemEventArgs e)
    {
        using var scope = _serviceProvider.CreateScope();
        var documentService = scope.ServiceProvider.GetRequiredService<DocumentService>();

        var path = GetRelativePath(e.FullPath);

        switch (e.ChangeType)
        {
            case WatcherChangeTypes.Changed:
                await documentService.UpdateHtmlAsync(path);
                break;
            case WatcherChangeTypes.Created:
                await documentService.GeneratorHtmlAsync(path);
                break;
            case WatcherChangeTypes.Deleted:
                await documentService.DeleteHtmlAsync(path);
                break;
        }
    }

    private string GetRelativePath(string fullPath)
    {
        var path = Path.GetRelativePath(FileHelper.GetMarkdownPath(), fullPath);

        return FileHelper.SharpPath(path);
    }

    private async Task OnRenamed(object source, RenamedEventArgs e)
    {
        using var scope = _serviceProvider.CreateScope();
        var documentService = scope.ServiceProvider.GetRequiredService<DocumentService>();

        if(IsTemporaryFile(e.Name ?? string.Empty))
        {
            return;
        }

        if(IsTemporaryFile(e.OldName ?? string.Empty))
        {
            await documentService.UpdateHtmlAsync(GetRelativePath(e.FullPath));
        }
        else
        {
            await documentService.DeleteHtmlAsync(GetRelativePath(e.OldFullPath));
            await documentService.GeneratorHtmlAsync(GetRelativePath(e.FullPath));
        }
    }

    private bool IsTemporaryFile(string fileName)
    {
        return fileName.EndsWith("~") || fileName.EndsWith(".tmp", StringComparison.OrdinalIgnoreCase);
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
