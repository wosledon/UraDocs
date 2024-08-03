using System.Net.Http.Json;
using UraDocs.Shared;

namespace UraDocs.Frontend.Services;

public class UraService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public UraService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<List<UraMenu>> GetMenusAsync()
    {
        var httpClient = _httpClientFactory.CreateClient("apiservice");

        List<UraMenu>? menus = null;

        await foreach (var menu in httpClient.GetFromJsonAsAsyncEnumerable<UraMenu>("api/ura/menu"))
        {
            if (menu is not null)
            {
                menus ??= new();
                menus.Add(menu);
            }
        }

        return menus ?? new List<UraMenu>();
    }
}
