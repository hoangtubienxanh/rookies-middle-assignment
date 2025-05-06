namespace Api.Models.Book;

public record BookListOptions
{
    public int PageIndex { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}