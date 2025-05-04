namespace Scribe.EntityFrameworkCore.Stores;

public class Category
{
    public Guid CategoryId { get; private init; }
    public required string Name { get; init; }
    public string? Slug { get; init; }
}