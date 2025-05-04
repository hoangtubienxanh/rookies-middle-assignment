namespace Scribe.Models;

public static class BookViewModelExtensions
{
    public static EntityFrameworkCore.Stores.Book ToEntity(this CreateBookRequest newBook)
    {
        var book = new EntityFrameworkCore.Stores.Book
        {
            Title = newBook.Title,
            Author = newBook.Author,
            Quantity = newBook.InputQuantity,
            CategoryId = newBook.CategoryId
        };

        return book;
    }

    public static EntityFrameworkCore.Stores.Book ToEntity(this UpdateBookRequest modifiedBook)
    {
        var book = new EntityFrameworkCore.Stores.Book
        {
            Title = modifiedBook.Title,
            Author = modifiedBook.Author,
            Quantity = modifiedBook.InputQuantity,
            CategoryId = modifiedBook.CategoryId
        };

        return book;
    }

    public static Book ToView(this EntityFrameworkCore.Stores.Book entity)
    {
        var book = new Book
        {
            Title = entity.Title,
            Author = entity.Author,
            Category = new Category { Name = entity.Category?.Name, Slug = entity.Category?.Slug },
            Available = true // TODO
        };

        return book;
    }
}