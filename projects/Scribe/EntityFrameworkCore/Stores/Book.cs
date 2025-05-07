namespace Scribe.EntityFrameworkCore.Stores;

public class Book
{
    public Guid BookId { get; set; }
    public required string Title { get; set; }
    public required string Author { get; set; }
    public int Quantity { get; set; }

    // Foreign key property
    public Guid? CategoryId { get; set; }
    public Category? Category { get; set; }

    // Ignored property
    public int? LendingQuantity { get; set; }

    public int AvailableQuantity()
    {
        if (!LendingQuantity.HasValue)
        {
            throw new ArgumentNullException(nameof(LendingQuantity));
        }

        return Quantity - LendingQuantity.Value;
    }
}