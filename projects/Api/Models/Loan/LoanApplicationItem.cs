using Api.Models.Book;

namespace Api.Models.Loan;

public record LoanApplicationItem
{
    public Guid Id { get; init; }
    public required string Status { get; init; }
    public DateTimeOffset Created { get; init; }
    public IReadOnlyList<BookItem> Items { get; init; }
}