namespace Scribe.EntityFrameworkCore.Stores;

public class Review
{
    public Guid Id { get; private init; }
    public Guid BookId { get; init; }
    public Guid UserId { get; init; }
    public bool Recommended { get; set; }
    public string? Comment { get; set; }
    public DateTimeOffset ReviewDate { get; set; }
}