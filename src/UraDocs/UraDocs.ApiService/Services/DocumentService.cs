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
    private readonly UnitOfWork _db;

    public DocumentService(
        IWebHostEnvironment webHost
        , SnowflakeGeneratorService snowflakeGeneratorService
        , UnitOfWork db)
    {
        _webHost = webHost;
        _snowflakeGeneratorService = snowflakeGeneratorService;
        _db = db;
    }

    private string CheckMarkdownPath(string markdownPath)
    {
        var path = FileHelper.GetMarkDownPath(markdownPath);
        if (!File.Exists(path))
            throw new FileNotFoundException("El archivo no existe", markdownPath);

        return path;
    }

    private async Task<(FileHash? fileHash,string relationMarkdownPath)> GetFileHashAsync(string markdownPath)
    {
        var relationMarkdownPath = CheckMarkdownPath(markdownPath);

        var fileHash = await _db.Query<FileHash>().FirstOrDefaultAsync(x => x.FilePath.ToLower() == relationMarkdownPath.ToLower());

        return (fileHash, relationMarkdownPath);
    }

    public async Task GeneratorHtmlAsync(string markdownPath)
    {
        var (fileHash, relationMarkdownPath) = await GetFileHashAsync(markdownPath);

        if (fileHash is not null) return;

        var markdown = File.ReadAllText(relationMarkdownPath);

        var html = Markdig.Markdown.ToHtml(markdown);

        var htmlId = _snowflakeGeneratorService.GetId();

        var hash = new FileHash
        {
            FilePath = markdownPath,
            FileName = Path.GetFileName(markdownPath),
            Hash = relationMarkdownPath.GetFileHash(),
            Html = $"{htmlId}.html"
        };

        var path = GetHtmlPath(hash.Html);

        if (!File.Exists(path))
        {
            File.Create(path).Close();
        }

        await WriteTextAsync(path, html);
        await _db.InsertAsync(hash);
    }

    private async Task WriteTextAsync(string path, string text)
    {
        await File.WriteAllTextAsync(path, text, Encoding.UTF8);
    }

    public async Task UpdateHtmlAsync(string markdownPath)
    {
        var (fileHash, relationMarkdownPath) = await GetFileHashAsync(markdownPath);

        if (fileHash is null) return;

        var hash = relationMarkdownPath.GetFileHash();

        if (hash == fileHash.Hash)
        {
            return;
        }
        
        var markdown = File.ReadAllText(relationMarkdownPath);

        var html = Markdig.Markdown.ToHtml(markdown);

        if (fileHash == null)
        {
            await GeneratorHtmlAsync(markdownPath);
            return;
        }

        var path = GetHtmlPath(fileHash.Html);

        if (!File.Exists(path))
        {
            File.Create(path);
        }

        await WriteTextAsync(path, html);
        await _db.UpdateAsync(fileHash);
    }

    public async Task DeleteHtmlAsync(string markdownPath)
    {
        var (fileHash, relationMarkdownPath) = await GetFileHashAsync(markdownPath);

        if (fileHash == null)
            return;

        var path = GetHtmlPath(fileHash.Html);

        if (File.Exists(path))
        {
            File.Delete(path);
        }

        await _db.DeleteAsync(fileHash);
    }

    private string GetHtmlPath(string htmlName)
    {
        var folder = Path.Combine(_webHost.WebRootPath, Contas.HtmlPath);

        if(!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        var htmlPath = Path.Combine(folder, htmlName);
        return htmlPath;
    }
}
