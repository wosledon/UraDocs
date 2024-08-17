using System.Collections.Concurrent;

namespace UraDocs.Shared;

public static class UraMenuExtensions
{
    public static List<UraMenuTree> ToUraMenuTreeList(this ConcurrentDictionary<string, UraMenu> menus)
    {
        //var menuDict = menus.ToDictionary(m => m.Path, m => new UraMenuTree
        //{
        //    Path = m.Path,
        //    Name = m.Name,
        //    HtmlDoc = m.HtmlDoc
        //});

        var menuDict = new Dictionary<string, UraMenuTree>();
        foreach (var menu in menus)
        {
            menuDict[menu.Key] = new UraMenuTree
            {
                Path = menu.Value.Path,
                Name = menu.Value.Name,
                HtmlDoc = menu.Value.HtmlDoc
            };
        }

        var rootMenus = new List<UraMenuTree>();

        foreach (var menu in menuDict.Values)
        {
            var parentPath = GetParentPath(menu.Path);
            if (menuDict.TryGetValue(parentPath, out var parentMenu))
            {
                parentMenu.Children.Add(menu);
            }
            else
            {
                rootMenus.Add(menu);
            }
        }

        return rootMenus;
    }

    private static string GetParentPath(string path)
    {
        var lastIndex = path.LastIndexOf('/');
        return lastIndex > 0 ? path.Substring(0, lastIndex) : string.Empty;
    }
}
