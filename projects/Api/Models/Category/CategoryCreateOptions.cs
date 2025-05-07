using System.ComponentModel.DataAnnotations;

namespace Api.Models.Category;

public record CategoryCreateOptions
{
    [Required] [Length(3, 500)] public required string Name { get; init; }
}