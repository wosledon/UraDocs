using UraDocs.ApiService.Extensions;
using UraDocs.ApiService.Helpers;
using UraDocs.Shared;

namespace UraDocs.ApiService.Services;

public class MenuService
{
    private static List<UraMenu> _menus = new();

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

        _menus = menus;
    }

    public async Task DeleteUraMenuAsync(UraMenu menu)
    {
        _menus = _menus.Where(x => x.Path != menu.Path).ToList();

        await UpdateUraMenuAsync(_menus);
    }

    public async Task InsertUraMenuAsync(UraMenu menu)
    {
        _menus.Add(menu);
        await UpdateUraMenuAsync(_menus);
    }
}
