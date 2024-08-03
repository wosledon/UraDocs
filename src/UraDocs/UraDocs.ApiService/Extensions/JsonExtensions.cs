using Newtonsoft.Json;

namespace UraDocs.ApiService.Extensions;

public static class JsonExtensions
{
    public static string ToJson(this object? value)
    {
        if (value is null) return string.Empty;

        try
        {
            return JsonConvert.SerializeObject(value);
        }
        catch
        {
            return string.Empty;
        }
    }

    public static T? ToObject<T>(this string? json)
    {
        if(string.IsNullOrWhiteSpace(json)) return default;

        try
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
        catch
        {
            return default;
        }
    }
}
