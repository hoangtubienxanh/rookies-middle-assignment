using System.ComponentModel.DataAnnotations;

namespace Api.Models.Loan;

public record LoanApplicationCreateOptions
{
    [Required] [Length(1, 5)] public required List<Guid> Items { get; init; }

    public IReadOnlyList<Guid> ValidatedItems => Items.Distinct().ToList();
}