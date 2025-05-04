using Scribe.EntityFrameworkCore;
using Scribe.EntityFrameworkCore.Stores;

namespace Scribe.Services;

public interface IBookManager
{
    public Task<Book> CreateAsync(Book newBook);
    public Task UpdateAsync(Book existingBook, Book modifiedBook);
    public Task SetBookArchivedAsync(Book existingBook);
}

public class BookManager(ScribeContext db) : IBookManager
{
    public async Task<Book> CreateAsync(Book newBook)
    {
        db.Books.Add(newBook);
        await db.SaveChangesAsync();
        return newBook;
    }

    public async Task UpdateAsync(Book existingBook, Book modifiedBook)
    {
        if (existingBook is not { Archived: true })
        {
            throw new InvalidOperationException();
        }

        existingBook.Title = modifiedBook.Title;
        existingBook.Author = modifiedBook.Author;
        existingBook.Quantity = modifiedBook.Quantity;
        existingBook.CategoryId = modifiedBook.CategoryId;

        await db.SaveChangesAsync();
    }

    public async Task SetBookArchivedAsync(Book existingBook)
    {
        existingBook.Archived = true;
        await db.SaveChangesAsync();
    }

    public async Task AddLoanItemsAsync(Loan loan, params List<Book> books)
    {
        if (books.Count > 5)
        {
            throw new ArgumentOutOfRangeException(
                nameof(books),
                books.Count,
                "Cannot loan more than 5 items at once.");
        }

        var hasUnavailableItems = books.Any(b => b.AvailableQuantity <= 0 || b.Archived);
        if (hasUnavailableItems)
        {
            throw new InvalidOperationException("One or more books are not available to loan.");
        }

        loan.Books.AddRange(books);
        await db.SaveChangesAsync();
    }

    public async Task ExtendLoanItemAsync(Loan loan, Book book)
    {
        if (!loan.ProcessingDate.HasValue || loan.Status != LoanStatus.Approved)
        {
            throw new InvalidOperationException("Loan has not been approved.");
        }

        var item = loan.LoanItems.FirstOrDefault(li => li.BookId == book.BookId);
        if (item is null)
        {
            throw new InvalidOperationException("Book is not in this loan.");
        }

        item.ReturnDate = loan.ProcessingDate.Value.AddDays(14);
        item.ExtensionCount += 1;
        await db.SaveChangesAsync();
    }
}