using LiteDB;

namespace UraDocs.ApiService.Domain;

public class FileHash
{
    [BsonId]
    public string FilePath { get; set; } = string.Empty;
    public string Hash { get; set; } = string.Empty;
}
