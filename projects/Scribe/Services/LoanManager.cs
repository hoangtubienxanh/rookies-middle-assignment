using Scribe.EntityFrameworkCore;
using Scribe.EntityFrameworkCore.Stores;

namespace Scribe.Services;

public interface ILoanManager
{
    public Task<Loan> CreateAsync(Guid borrowerId, Loan newLoan);
    public Task AddLoanItemsAsync(Loan loan, params List<Book> books);
    public Task ExtendLoanItemAsync(Loan loan, Book book);
}

public class LoanManager(TimeProvider timeProvider, ScribeContext db) : ILoanManager
{
    public async Task<Loan> CreateAsync(Guid borrowerId, Loan newLoan)
    {
        newLoan.BorrowerId = borrowerId;
        newLoan.ProcessingDate = timeProvider.GetUtcNow();
        db.Loans.Add(newLoan);
        await db.SaveChangesAsync();
        return newLoan;
    }

    public async Task AddLoanItemsAsync(Loan loan, params List<Book> books)
    {
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