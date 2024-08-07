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
        var menus = await FileHelper.GetMenuAsync();

        foreach(var menu in menus)
        {
            //await PrepareMenuAsync(menu);
        }

        return Ok(menus);
    }

    private async Task PrepareMenuAsync(UraMenuTree menu)
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
