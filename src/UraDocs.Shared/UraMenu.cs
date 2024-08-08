namespace UraDocs.Shared;

public class UraMenu
{
    public string Path { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Hash { get; set; } = string.Empty;
    public string HtmlDoc { get; set; } = string.Empty;
}

public class UraMenuTree: UraMenu
{
    public List<UraMenuTree> Children { get; set; } = new();
}

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

public class UraProject
{
    public string Title { get; set; } = "Ura";
    public string Url { get; set; } = "https://ura.app";
    public string Description { get; set; } = "Ura is a simple, fast, and lightweight web framework for .NET.";

    public string Version { get; set; } = "1.0.0";
    public string Author { get; set; } = "Aspire";
    public string AuthorUrl { get; set; } = "https://aspireapp.com";
    public string License { get; set; } = "MIT";
    public string LicenseUrl { get; set; } = "https://opensource.org/licenses/MIT";
    public string Repository { get; set; } = "";
    public string Support { get; set; } = string.Empty;
    public string SupportBy { get; set; } = string.Empty;
}

public class UraRepository
{
    public enum RepoType
    {
        GitHub,
        GitLab,
        Gitee
    }

    public RepoType Type { get; set; } = RepoType.GitHub;
    public string RepositoryUrl { get; set; } = string.Empty;
}

public class UraDocument
{
    public UraProject Ura { get; set; } = new();

    public UraMenu Menu { get; set; } = new();
}