using System.Collections.Concurrent;
using System.Text;
using UraDocs.ApiService.Extensions;
using UraDocs.ApiService.Helpers;
using UraDocs.Shared;

namespace UraDocs.ApiService.Services;

public class MenuService
{
    private static readonly ConcurrentDictionary<string, UraMenu> _menus = new(StringComparer.OrdinalIgnoreCase);

    public MenuService()
    {
        LoadMenus();
    }

    private void LoadMenus()
    {
        var menus = GetPhyUraMenus();

        _menus.Clear();
        foreach (var menu in menus)
        {
            _menus.TryAdd(menu.Path, menu);
        }
    }

    public void ReloadMenus()
    {
        LoadMenus();
    }

    private void UpdateMenus(List<UraMenu> menus)
    {
        _menus.Clear();
        foreach (var menu in menus)
        {
            _menus.TryAdd(menu.Path, menu);
        }
    }

    private void AddMenu(UraMenu menu)
    {
        _menus.TryAdd(menu.Path, menu);
    }

    private string GetMenuPath()
    {
        var menuPath = FileHelper.GetUraMenuPath();

        if (!File.Exists(menuPath))
        {
            throw new FileNotFoundException("El archivo no existe", menuPath);
        }

        return menuPath;
    }

    public async Task<List<UraMenu>> GetUraMenuAsync()
    {
        if (_menus.Any())
        {
            return _menus.Values.ToList();
        }

        var menuPath = GetMenuPath();
        var menu = await ReadFromFileAsync(menuPath);

        return menu.ToObject<List<UraMenu>>() ?? new List<UraMenu>();
    }

    public async Task<UraMenu?> FirstOrDefaultAsync(Func<UraMenu, bool> predicate)
    {
        var menus = await GetUraMenuAsync();

        return menus.FirstOrDefault(predicate);
    }

    public async Task UpdateUraMenuAsync()
    {
        var menuPath = GetMenuPath();

        await WriteToFileAsync(menuPath, _menus.Values.ToJson());
    }

    public async Task UpdateUraMenuAsync(UraMenu menu)
    {
        _menus[menu.Path] = menu;

        await UpdateUraMenuAsync();
    }

    public async Task DeleteUraMenuAsync(UraMenu menu)
    {
        _menus.TryRemove(menu.Path, out _);

        await UpdateUraMenuAsync();
    }

    public async Task InsertUraMenuAsync(UraMenu menu)
    {
        _menus.TryAdd(menu.Path, menu);
        await UpdateUraMenuAsync();
    }

    public List<UraMenuTree> GetMenuTree()
    {
        return _menus.ToUraMenuTreeList();
    }

    private List<string> GetMenuPaths()
    {
        var markdownPath = FileHelper.GetMarkdownPath();

        return Directory.GetFiles(markdownPath, "*.md", SearchOption.AllDirectories).ToList();
    }

    private List<UraMenu> GetPhyUraMenus()
    {
        var paths = GetMenuPaths();

        return paths.Select(x =>
        {
            var relativePath = Path.GetRelativePath(FileHelper.GetMarkdownPath(), x);

            return new UraMenu
            {
                Path = relativePath,
                Name = Path.GetFileNameWithoutExtension(x),
                Hash = x.GetFileHash(),
            };
        }).ToList();
    }

    private static async Task WriteToFileAsync(string filePath, string content)
    {
        byte[] encodedText = Encoding.UTF8.GetBytes(content);

        await using (FileStream sourceStream = new(filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
        {
            await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
        }
    }

    private static async Task<string> ReadFromFileAsync(string filePath)
    {
        await using (FileStream sourceStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true))
        {
            StringBuilder sb = new();

            byte[] buffer = new byte[0x1000];
            int numRead;
            while ((numRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
            {
                string text = Encoding.UTF8.GetString(buffer, 0, numRead);
                sb.Append(text);
            }

            return sb.ToString();
        }
    }
}