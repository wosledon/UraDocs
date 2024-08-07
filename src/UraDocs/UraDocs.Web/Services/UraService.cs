using UraDocs.Shared;

namespace UraDocs.Web.Services;

public class UraService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public UraService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<List<UraMenuTree>> GetMenusAsync()
    {
        var httpClient = _httpClientFactory.CreateClient("apiservice");

        List<UraMenuTree>? menus = null;

        await foreach (var menu in httpClient.GetFromJsonAsAsyncEnumerable<UraMenuTree>("api/ura/menu"))
        {
            if (menu is not null)
            {
                menus ??= new();
                menus.Add(menu);
            }
        }

        return menus ?? new List<UraMenuTree>();
    }
}
