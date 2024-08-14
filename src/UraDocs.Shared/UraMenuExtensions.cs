namespace UraDocs.Shared;

public static class UraMenuExtensions
{
    public static List<UraMenuTree> ToUraMenuTreeList(this List<UraMenu> menus)
    {
        var menuDict = menus.ToDictionary(m => m.Path, m => new UraMenuTree
        {
            Path = m.Path,
            Name = m.Name,
            HtmlDoc = m.HtmlDoc
        });

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
