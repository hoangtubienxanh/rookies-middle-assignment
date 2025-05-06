namespace Api.Models.Category;

public record CategoryListOptions
{
    public int PageIndex { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}