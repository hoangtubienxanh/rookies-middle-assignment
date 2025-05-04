namespace Scribe.EntityFrameworkCore.Stores;

public class Book
{
    private int _quantity;
    public Guid BookId { get; private init; }
    public required string Title { get; set; }
    public required string Author { get; set; }

    public int Quantity
    {
        get => _quantity;
        set
        {
            if (value >= BorrowedQuantity)
            {
                _quantity = value;
            }
            else
            {
                throw new ArgumentException(null, nameof(value));
            }
        }
    }

    public bool Archived { get; set; }

    // Computed on select
    public int BorrowedQuantity { get; private init; }
    public int AvailableQuantity => Quantity - BorrowedQuantity;

    // Optional reference navigation to principal
    public Guid? CategoryId { get; set; }
    public Category? Category { get; private init; }

    // Concurrency tokens
    public Guid Version { get; private set; }
}