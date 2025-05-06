namespace Scribe.EntityFrameworkCore.Stores;

public class Category
{
    public Guid CategoryId { get; private init; }
    public required string Name { get; set; }
    public string? Slug { get; set; }
}