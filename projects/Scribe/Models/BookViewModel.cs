namespace Scribe.Models;

public record Book
{
    public Guid Id { get; init; }
    public required string Title { get; init; }
    public required string Author { get; init; }
    public required Category Category { get; init; }
    public bool Available { get; init; }
}

public record Category
{
    public string? Name { get; init; }
    public string? Slug { get; init; }
}

public record BookQueryFilter
{
    public static bool TryParse(string? value, IFormatProvider? provider,
        out BookQueryFilter? filter)
    {
        filter = null;
        return true;
    }
}

public record CreateBookRequest
{
    public required string Title { get; init; }
    public required string Author { get; init; }
    public int InputQuantity { get; init; }
    public Guid CategoryId { get; init; }
}

public record UpdateBookRequest
{
    public required string Title { get; init; }
    public required string Author { get; init; }
    public int InputQuantity { get; init; }
    public Guid CategoryId { get; init; }
}