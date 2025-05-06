using Api.Models;
using Api.Models.Book;
using Api.Services;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Scribe.EntityFrameworkCore;

namespace Api.Endpoints;

public static class BookEndpoint
{
    public static RouteGroupBuilder MapBookEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/books");

        var authorizedGroup = group.MapGroup("")
            .RequireAuthorization("all_access");

        group.MapGet("/", GetAllBooks)
            .WithName("GetAllBooks")
            .WithParameterValidation();

        group.MapGet("/{id:guid}", GetBookById)
            .WithName("GetBookById");

        authorizedGroup.MapPut("/{id:guid}", UpdateBook)
            .WithName("UpdateBook")
            .WithParameterValidation();

        authorizedGroup.MapPost("/", CreateBook)
            .WithName("CreateBook")
            .WithParameterValidation();

        authorizedGroup.MapDelete("/{id:guid}", DeleteBook)
            .WithName("DeleteBook");

        return group;
    }

    public static async Task<Ok<PaginatedItems<BookItem>>> GetAllBooks(
        [FromBody] BookListOptions options,
        [FromServices] IBookManager manager)
    {
        var bookItems = await manager.GetAllBooksAsync(options);
        return TypedResults.Ok(bookItems);
    }

    public static async Task<Results<Ok<BookItem>, NotFound<ProblemDetails>>> GetBookById(
        [FromRoute] Guid id,
        [FromServices] IBookManager manager)
    {
        var bookItem = await manager.GetBookAsync(id);
        return bookItem is null ? CreateNotFoundProblemDetail(id) : TypedResults.Ok(bookItem);
    }

    public static async Task<Results<Ok<BookItem>, NotFound<ProblemDetails>>> UpdateBook(
        [FromRoute] Guid id,
        [FromBody] BookUpdateOptions options,
        [FromServices] TimeProvider timeProvider,
        [FromServices] ScribeContext context,
        [FromServices] IBookManager manager)
    {
        var book = await context.Books.SingleOrDefaultAsync(b => b.BookId == id);
        if (book is null)
        {
            return CreateNotFoundProblemDetail(id);
        }

        await manager.UpdateAsync(book, options);
        return TypedResults.Ok(book.AsBookItem());
    }


    public static async Task<Ok<BookItem>> CreateBook(BookCreateOptions options,
        [FromServices] IBookManager manager)
    {
        var book = await manager.CreateAsync(options);
        return TypedResults.Ok(book.AsBookItem());
    }

    public static async Task<Results<Ok, NotFound<ProblemDetails>>> DeleteBook(
        Guid id,
        [FromServices] ScribeContext context,
        [FromServices] IBookManager manager)
    {
        var book = await context.Books.SingleOrDefaultAsync(b => b.BookId == id);
        if (book is null)
        {
            return CreateNotFoundProblemDetail(id);
        }

        await manager.DeleteAsync(book);
        return TypedResults.Ok();
    }

    private static NotFound<ProblemDetails> CreateNotFoundProblemDetail(Guid id)
    {
        return TypedResults.NotFound(new ProblemDetails { Detail = $"Item with id {id} not found." });
    }
}