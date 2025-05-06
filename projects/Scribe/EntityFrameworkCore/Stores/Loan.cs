namespace Scribe.EntityFrameworkCore.Stores;

public class Loan
{
    public Guid LoanId { get; private init; }
    public DateTimeOffset LoanDate { get; set; }
    public DateTimeOffset DueDate { get; set; }
    public DateTimeOffset? ReturnDate { get; set; }

    // Foreign key property
    public required Guid BookId { get; set; }
    public required Guid ApplicantId { get; set; }
    public required Guid LoanApplicationId { get; set; }

    // Reference navigation to principal.
    public ScribeUser Applicant { get; private init; } = null!;
    public Book Book { get; private init; } = null!;
    public LoanApplication LoanApplication { get; private init; } = null!;
}