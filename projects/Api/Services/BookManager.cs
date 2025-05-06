using Api.Models;
using Api.Models.Book;

using Microsoft.EntityFrameworkCore;

using Scribe.EntityFrameworkCore;
using Scribe.EntityFrameworkCore.Stores;

namespace Api.Services;

public class BookManager(TimeProvider timeProvider, ScribeContext context) : IBookManager
{
    public async Task<Book> CreateAsync(BookCreateOptions options)
    {
        var book = new Book
        {
            Title = options.Title,
            Author = options.Author,
            Quantity = options.InputQuantity,
            CategoryId = options.CategoryId,
            LendingQuantity = 0
        };

        context.Books.Add(book);
        await context.SaveChangesAsync();
        return book;
    }

    public async Task<Book> UpdateAsync(Book book, BookUpdateOptions options)
    {
        await IncludeLendingQuantity(book);
        if (book.LendingQuantity > options.InputQuantity)
        {
            throw new InvalidOperationException("Cannot set quantity less than what's currently in use.");
        }

        book.Title = options.Title;
        book.Author = options.Author;
        book.CategoryId = options.CategoryId;
        book.Quantity = options.InputQuantity;

        await context.SaveChangesAsync();
        return book;
    }

    public async Task DeleteAsync(Book book)
    {
        try
        {
            context.Books.Remove(book);
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException("Cannot delete book that is currently in use.", ex);
        }
    }

    public async Task IncludeLendingQuantity(Book book)
    {
        book.LendingQuantity = await context.Loans
            .Where(loan => loan.BookId == book.BookId)
            .Where(loan => loan.ReturnDate == null || loan.DueDate > timeProvider.GetUtcNow())
            .CountAsync();
    }

    public async Task IncludeLendingQuantity(params List<Book> books)
    {
        var bookIds = books.Select(book => book.BookId);

        var activeLoansCount = await context.Loans
            .Where(loan => bookIds.Contains(loan.BookId))
            .Where(loan => loan.ReturnDate == null || loan.DueDate > timeProvider.GetUtcNow())
            .GroupBy(loan => loan.BookId)
            .Select(group => new { BookId = group.Key, LendingQuantity = group.Count() })
            .ToDictionaryAsync(result => result.BookId, result => result.LendingQuantity);

        foreach (var book in books)
        {
            book.LendingQuantity = activeLoansCount.TryGetValue(book.BookId, out var count) ? count : 0;
        }
    }

    public async Task<PaginatedItems<BookItem>> GetAllBooksAsync(BookListOptions options)
    {
        var totalBooks = await context.Books.CountAsync();

        var bookItems = await context.Books
            .OrderBy(b => b.BookId)
            .Skip(options.PageIndex * options.PageSize)
            .Take(options.PageSize)
            .Select(b => new BookItem { Id = b.BookId, Title = b.Title, Author = b.Author, CategoryId = b.CategoryId })
            .ToListAsync();

        return new PaginatedItems<BookItem>(options.PageIndex, options.PageSize, totalBooks, bookItems);
    }

    public async Task<BookItem?> GetBookAsync(Guid id)
    {
        var bookItem = await context.Books.Where(b => b.BookId == id)
            .Select(b => new BookItem { Id = b.BookId, Title = b.Title, Author = b.Author, CategoryId = b.CategoryId })
            .FirstOrDefaultAsync();
        return bookItem;
    }
}