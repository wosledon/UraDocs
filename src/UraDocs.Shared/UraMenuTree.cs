namespace UraDocs.Shared;

public class UraMenuTree : UraMenu
{
    public List<UraMenuTree> Children { get; set; } = new();
}
