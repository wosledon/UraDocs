using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UraDocs.ApiService.Domain;
using UraDocs.ApiService.Helpers;
using UraDocs.ApiService.Services;
using UraDocs.Shared;

namespace UraDocs.ApiService.Controllers;

public class UraController : ApiControllerBase
{
    private readonly MenuService _menuService;

    public UraController(MenuService menuService)
    {
        _menuService = menuService;
    }

    [HttpGet]
    public IActionResult MenuAsync()
    {
        return Ok(_menuService.GetMenuTree());
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
