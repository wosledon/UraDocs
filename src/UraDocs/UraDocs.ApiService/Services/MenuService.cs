using UraDocs.ApiService.Extensions;
using UraDocs.ApiService.Helpers;
using UraDocs.Shared;

namespace UraDocs.ApiService.Services;

public class MenuService
{
    private static List<UraMenu> _menus = new();

    static object _lock = new();

    private void LoadMenus()
    {
        lock(_lock)
        {
            if(!_menus.Any())
            {
                _menus = GetPhyUraMenus();
            }
        }
    }

    private void ReloadMenus()
    {
        lock(_lock)
        {
            _menus = GetPhyUraMenus();
        }
    }

    private void UpdateMenus(List<UraMenu> menus)
    {
        lock (_lock)
        {
            _menus = menus;
        }
    }

    private void AddMenu(UraMenu menu)
    {
        lock (_lock)
        {
            _menus.Add(menu);
        }
    }

    private string GetMenuPath()
    {
       var menuPath = FileHelper.GetUraMenuPath();

        if (!File.Exists(menuPath))
            throw new FileNotFoundException("El archivo no existe", menuPath);

        return menuPath;
    }
    public async Task<List<UraMenu>> GetUraMenuAsync()
    {
        if(_menus.Any())
            return _menus;

        var menuPath = GetMenuPath();

        var menu = await File.ReadAllTextAsync(menuPath);

        return menu.ToObject<List<UraMenu>>() ?? [];
    }

    public async Task UpdateUraMenuAsync(List<UraMenu> menus)
    {
        var menuPath = GetMenuPath();

        await File.WriteAllTextAsync(menuPath, menus.ToJson());

        UpdateMenus(menus);
    }

    public async Task DeleteUraMenuAsync(UraMenu menu)
    {
        var menus = _menus.Where(x => x.Path != menu.Path).ToList();

        await UpdateUraMenuAsync(menus);
    }

    public async Task InsertUraMenuAsync(UraMenu menu)
    {
        AddMenu(menu);
        await UpdateUraMenuAsync(_menus);
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

        return paths.Select(x => {
            return new UraMenu
            {
                Path = x,
                Name = Path.GetFileNameWithoutExtension(x),
                Hash = x.GetFileHash(),
            };
        }).ToList();
    }
}
 