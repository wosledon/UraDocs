namespace UraDocs.ApiService.Domain;

public class FileHash
{
    public Guid Id { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string Hash { get; set; } = string.Empty;
    public string Html { get; set; } = string.Empty;
}
