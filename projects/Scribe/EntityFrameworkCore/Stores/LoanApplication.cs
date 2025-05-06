namespace Scribe.EntityFrameworkCore.Stores;

public class LoanApplication
{
    public Guid LoanApplicationId { get; private init; }
    public LoanStatus Status { get; set; }
    public DateTimeOffset ApplicationDate { get; set; }
    public DateTimeOffset? DecisionDate { get; set; }

    // Foreign key property
    public Guid ApplicantId { get; set; }
    public Guid? ActorId { get; set; }

    // Skip navigation. Containing required reference navigation to principal
    public List<Book> ApplicationItems { get; } = [];

    // Collection navigation to dependent.
    public List<Loan> LendingItems { get; } = [];
}