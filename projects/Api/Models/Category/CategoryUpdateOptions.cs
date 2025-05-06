using System.ComponentModel.DataAnnotations;

namespace Api.Models.Category;

public record CategoryUpdateOptions
{
    [Required] [Length(6, 500)] public required string Name { get; init; }
}