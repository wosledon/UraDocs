using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UraDocs.ApiService.Domain;
using UraDocs.ApiService.Helpers;
using UraDocs.ApiService.Services;
using UraDocs.Shared;

namespace UraDocs.ApiService.Controllers;

public class UraController : ApiControllerBase
{
    private readonly UnitOfWork _db;

    public UraController(UnitOfWork db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> MenuAsync()
    {
        var menu = await FileHelper.GetMenuAsync();

        await PrepareMenuAsync(menu);

        return Ok(menu);
    }

    private async Task PrepareMenuAsync(UraMenu menu)
    {
        var path = FileHelper.SharpPath(menu.Path);

        var fileHash = await _db.Query<FileHash>().FirstOrDefaultAsync(x => x.FilePath.ToLower() == path.ToLower());

        menu.HtmlDoc = GetHtmlPath(fileHash?.Html);

        if(menu.Children is not null)
        {
            foreach (var child in menu.Children)
            {
                await PrepareMenuAsync(child);
            }
        }
    }

    private string GetHtmlPath(string? path)
    {
        if(string.IsNullOrWhiteSpace(path))
        {
            return string.Empty;
        }

        return $"{Contas.HtmlPath}/{path}";
    }

    [HttpGet]
    public async Task<IActionResult> ProjectAsync()
    {
        var project = await FileHelper.GetProjectAsync();
        return Ok(project);
    }
}
