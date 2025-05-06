using System.ComponentModel.DataAnnotations;

namespace Api.Models.Book;

public record BookUpdateOptions
{
    [Required] [Length(6, 500)] public required string Title { get; init; }
    [Required] [Length(6, 500)] public required string Author { get; init; }
    [Range(0, int.MaxValue)] public int InputQuantity { get; init; }
    public Guid CategoryId { get; init; }
}