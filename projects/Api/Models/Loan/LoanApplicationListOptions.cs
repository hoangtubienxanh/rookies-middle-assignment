namespace Api.Models.Loan;

public record LoanApplicationListOptions
{
    public int PageIndex { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}