using Markdig;
using Microsoft.EntityFrameworkCore;
using System.Text;
using UraDocs.ApiService.Domain;
using UraDocs.ApiService.Extensions;
using UraDocs.ApiService.Helpers;
using UraDocs.Shared;

namespace UraDocs.ApiService.Services;

public class DocumentService
{
    private readonly IWebHostEnvironment _webHost;
    private readonly SnowflakeGeneratorService _snowflakeGeneratorService;
    private readonly MenuService _menuService;
    public DocumentService(
        IWebHostEnvironment webHost
        , SnowflakeGeneratorService snowflakeGeneratorService
        , MenuService menuService)
    {
        _webHost = webHost;
        _snowflakeGeneratorService = snowflakeGeneratorService;
        _menuService = menuService;
    }

    private string CheckMarkdownPath(string markdownPath)
    {
        var path = FileHelper.GetMarkDownPath(markdownPath);
        if (!File.Exists(path))
            throw new FileNotFoundException("El archivo no existe", markdownPath);

        return path;
    }

    /// <summary>
    /// TODO: Implement this method
    /// </summary>
    /// <param name="menu"></param>
    /// <param name="markdownPath"></param>
    /// <returns></returns>
    private async Task<(UraMenu? menu, string relationMarkdownPath)> GetMenuAsync(string markdownPath)
    {
        var relationMarkdownPath = CheckMarkdownPath(markdownPath);

        var menu = await _menuService.FirstOrDefaultAsync(x => x.Path.ToLower() == markdownPath.ToLower());

        return (menu, relationMarkdownPath);
    }

    private string GetHtml(string markdown)
    {
        var htmlContent = Markdig.Markdown.ToHtml(markdown);

        htmlContent = htmlContent.Replace("<pre>", "<pre class=\"line-numbers\">");

        return $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <link rel=""stylesheet"" href=""https://cdn.jsdelivr.net/npm/prismjs@1.25.0/themes/prism.min.css"">
    <link rel=""stylesheet"" href=""https://cdn.jsdelivr.net/npm/prismjs@1.25.0/plugins/line-numbers/prism-line-numbers.min.css"">
    <link rel=""stylesheet"" href=""https://cdn.jsdelivr.net/npm/prismjs@1.25.0/plugins/toolbar/prism-toolbar.min.css"">
    <title>Document</title>
</head>
<style>
/* 自定义复制按钮样式 */
.prism-toolbar .toolbar-item {{
    background-color: #007BFF; /* 蓝色背景 */
    color: white; /* 白色文字 */
    border-radius: 5px; /* 圆角 */
    padding: 5px 10px; /* 内边距 */
    font-size: 14px; /* 字体大小 */
    cursor: pointer; /* 鼠标指针 */
}}

.prism-toolbar .toolbar-item:hover {{
    background-color: #0056b3; /* 悬停时的背景颜色 */
}}

/* 自定义代码块样式 */
pre[class*=""language-""] {{
    background: #f5f5f5; /* 浅色背景 */
    color: #333; /* 深色文字 */
    border-radius: 5px; /* 圆角 */
    padding: 15px; /* 内边距 */
    overflow: auto; /* 滚动条 */
    font-size: 14px; /* 字体大小 */
    line-height: 1.5; /* 行高 */
}}

/* 自定义行号样式 */
pre[class*=""language-""].line-numbers {{
    padding-left: 3.8em; /* 为行号留出空间 */
}}

pre[class*=""language-""].line-numbers .line-numbers-rows {{
    border-right: 1px solid #ccc; /* 行号分隔线 */
}}

pre[class*=""language-""].line-numbers .line-numbers-rows > span:before {{
    color: #999; /* 行号颜色 */
}}
</style>
<body>
    {htmlContent}
    <script src=""https://cdn.jsdelivr.net/npm/prismjs@1.25.0/prism.min.js""></script>
    <script src=""https://cdn.jsdelivr.net/npm/prismjs@1.25.0/components/prism-core.min.js""></script>
    <script src=""https://cdn.jsdelivr.net/npm/prismjs@1.25.0/plugins/autoloader/prism-autoloader.min.js""></script>
    <script src=""https://cdn.jsdelivr.net/npm/prismjs@1.25.0/plugins/line-numbers/prism-line-numbers.min.js""></script>
    <script src=""https://cdn.jsdelivr.net/npm/prismjs@1.25.0/plugins/toolbar/prism-toolbar.min.js""></script>
    <script src=""https://cdn.jsdelivr.net/npm/prismjs@1.25.0/plugins/copy-to-clipboard/prism-copy-to-clipboard.min.js""></script>
    <script>
        document.addEventListener('DOMContentLoaded', (event) => {{
            Prism.highlightAll();
        }});
    </script>
</body>
</html>";
    }

    public async Task GeneratorHtmlAsync(string markdownPath)
    {
        var (fileHash, relationMarkdownPath) = await GetMenuAsync(markdownPath);

        if (fileHash is not null) return;

        var markdown = await ReadTextAsync(relationMarkdownPath);

        var html = GetHtml(markdown);

        var htmlId = _snowflakeGeneratorService.GetId();

        if (!relationMarkdownPath.TryGetFileHash(out var hashString))
        {
            return;
        }

        var menu = new UraMenu
        {
            Path = markdownPath,
            Name = Path.GetFileNameWithoutExtension(markdownPath),
            Hash = hashString,
            HtmlDoc = $"{htmlId}.html"
        };

        var path = GetHtmlPath(menu.HtmlDoc);

        if (!File.Exists(path))
        {
            File.Create(path).Close();
        }

        await WriteTextAsync(path, html);
        // await _db.InsertAsync(hash);
        await _menuService.InsertUraMenuAsync(menu);
    }

    private async Task<bool> WriteTextAsync(string path, string text)
    {
        try
        {
            await File.WriteAllTextAsync(path, text, Encoding.UTF8);

            return true;
        }
        catch
        {
            return false;
        }
    }

    private async Task<string> ReadTextAsync(string path)
    {
        try
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader reader = new StreamReader(fileStream))
            {
                return await reader.ReadToEndAsync();
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine($"无法读取文件: {ex.Message}");
            return string.Empty;
        }
    }

    public async Task UpdateHtmlAsync(string markdownPath)
    {
        var (menu, relationMarkdownPath) = await GetMenuAsync(markdownPath);

        if (menu == null)
        {
            await GeneratorHtmlAsync(markdownPath);
            return;
        }

        if (!relationMarkdownPath.TryGetFileHash(out var hash))
        {
            return;
        }

        if (hash == menu.Hash)
        {
            return;
        }

        var markdown = await ReadTextAsync(relationMarkdownPath);

        var html = GetHtml(markdown);

        var path = GetHtmlPath(menu.HtmlDoc);

        if (!File.Exists(path))
        {
            File.Create(path);
        }

        if (await WriteTextAsync(path, html))
        {
            await _menuService.UpdateUraMenuAsync(menu);
        }
    }

    public async Task DeleteHtmlAsync(string markdownPath)
    {
        var (menu, relationMarkdownPath) = await GetMenuAsync(markdownPath);

        if (menu == null)
            return;

        var path = GetHtmlPath(menu.HtmlDoc);

        if (File.Exists(path))
        {
            File.Delete(path);
        }

        await _menuService.DeleteUraMenuAsync(menu);
    }

    private string GetHtmlPath(string htmlName)
    {
        var folder = Path.Combine(_webHost.WebRootPath, Contas.HtmlPath);

        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        var htmlPath = Path.Combine(folder, htmlName);
        return htmlPath;
    }
}
