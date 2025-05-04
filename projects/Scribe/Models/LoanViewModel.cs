namespace Scribe.Models;

public record Loan
{
}

public record CreateLoanRequest
{
    public List<Guid> LoanItemOptions { get; init; }
}