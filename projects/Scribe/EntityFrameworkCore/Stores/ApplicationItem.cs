namespace Scribe.EntityFrameworkCore.Stores;

public class ApplicationItem
{
    public Guid ApplicationItemId { get; private init; }
    public Guid LoanApplicationId { get; set; }
    public Guid BookId { get; set; }

    public Book Book { get; private init; } = null!;
}