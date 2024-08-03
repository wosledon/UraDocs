using UraDocs.ApiService.Extensions;
using UraDocs.Shared;

namespace UraDocs.ApiService.Helpers;

public class FileHelper
{
    public static string GetMarkdownPath()
    {
        return Path.Combine(Directory.GetCurrentDirectory(), Contas.MarkdownPath);
    }

    public static string GetMarkDownPath(string path)
    {
        return Path.Combine(Contas.MarkdownPath, path.TrimStart(new[] {'.', '/'}));
    }

    public static string GetHtmlPath()
    {
        return Path.Combine(Directory.GetCurrentDirectory(), Contas.HtmlPath);
    }

    public static string GetUraProjectPath()
    {
        return Path.Combine(Directory.GetCurrentDirectory(), Contas.UraProjectPath);
    }

    public static string GetUraMenuPath()
    {
        return Path.Combine(Directory.GetCurrentDirectory(), Contas.UraMenuPath);
    }

    public static async Task<List<UraMenu>> GetMenuAsync()
    {
        var path = GetUraMenuPath();

        return await ReadAndCheckAsync<List<UraMenu>>(path);
    }

    public static async Task<UraProject> GetProjectAsync()
    {
        var path = GetUraProjectPath();

        return await ReadAndCheckAsync<UraProject>(path);
    }

    private static async Task SaveAsync(string path, object value)
    {
        var json = value.ToJson();

        await File.WriteAllTextAsync(path, json);
    }

    private static async Task<T> ReadAndCheckAsync<T>(string path)
        where T:new()
    {
        if (!File.Exists(path))
        {
            var obj = new T();
            await SaveAsync(path, obj);

            return obj;
        }

        var json = await File.ReadAllTextAsync(path);

        return json.ToObject<T>() ?? new T();
    }

    public static string SharpPath(string path)
    {
        var regPath =  path.Replace("//", "/").Replace("\\", "/");

        return regPath.StartsWith("./") ? regPath : $"./{regPath}";
    }
}
