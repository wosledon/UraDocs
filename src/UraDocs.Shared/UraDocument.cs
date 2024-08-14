namespace UraDocs.Shared;

public class UraDocument
{
    public UraProject Ura { get; set; } = new();

    public UraMenu Menu { get; set; } = new();
}