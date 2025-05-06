using Api.Models;
using Api.Models.Book;

using Scribe.EntityFrameworkCore.Stores;

namespace Api.Services;

public interface IBookManager
{
    public Task<Book> CreateAsync(BookCreateOptions options);
    public Task<Book> UpdateAsync(Book book, BookUpdateOptions options);
    public Task DeleteAsync(Book book);
    public Task IncludeLendingQuantity(Book book);
    public Task IncludeLendingQuantity(params List<Book> books);
    public Task<PaginatedItems<BookItem>> GetAllBooksAsync(BookListOptions options);
    public Task<BookItem?> GetBookAsync(Guid id);
}