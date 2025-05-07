namespace Api.Models.Book;

public static class BookMapperExtensions
{
    public static BookItem AsBookItem(this Scribe.EntityFrameworkCore.Stores.Book book)
    {
        return new BookItem
        {
            Id = book.BookId,
            Title = book.Title,
            Author = book.Author,
            CategoryId = book.CategoryId,
            Category = book.Category?.Name,
            Quantity = book.Quantity
        };
    }
}