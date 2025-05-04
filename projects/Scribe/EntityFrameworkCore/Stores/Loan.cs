namespace Scribe.EntityFrameworkCore.Stores;

public class Loan
{
    // Skip navigation
    public List<LoanItem> LoanItems = [];
    public Guid LoanId { get; private init; }
    public LoanStatus Status { get; set; } = LoanStatus.Open;
    public DateTimeOffset SubmissionDate { get; set; }
    public DateTimeOffset? ProcessingDate { get; set; }

    // Reference navigation to principal
    public Guid BorrowerId { get; set; }
    public Guid? ActorId { get; private init; }

    // Collection navigation containing principal
    public List<Book> Books { get; } = [];
}