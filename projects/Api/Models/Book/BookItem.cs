namespace Api.Models.Book;

public record BookItem
{
    public Guid Id { get; init; }
    public required string Title { get; init; }
    public required string Author { get; init; }
    public Guid? CategoryId { get; init; }
    public string? Category { get; init; }
    public required int Quantity { get; init; }
}