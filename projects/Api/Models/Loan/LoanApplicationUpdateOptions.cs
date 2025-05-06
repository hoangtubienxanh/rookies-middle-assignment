namespace Api.Models.Loan;

public record LoanApplicationUpdateOptions
{
    public string? Status { get; init; }
}