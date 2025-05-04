namespace Scribe.EntityFrameworkCore.Stores;

public class LoanItem
{
    public DateTimeOffset ReturnDate { get; set; }
    public int ExtensionCount { get; set; }
    public Guid LoanId { get; set; }
    public Guid BookId { get; set; }
}