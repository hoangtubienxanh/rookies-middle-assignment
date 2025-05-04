using Scribe.EntityFrameworkCore;
using Scribe.Models;

namespace Scribe.Services;

public interface IBookQueries
{
    Task<PaginatedItems<Book>> GetAllBooksAsync(BookQueryFilter filter);
    Task<Book?> GetBookAsync(Guid id);
}

public class BookQueries(ScribeContext context) : IBookQueries
{
    public async Task<PaginatedItems<Book>> GetAllBooksAsync(BookQueryFilter filter)
    {
        throw new NotImplementedException();
    }

    public async Task<Book?> GetBookAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}