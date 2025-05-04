using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using Scribe.EntityFrameworkCore;
using Scribe.Models;
using Scribe.Services;

namespace Api.Endpoints;

public static class BookEndpoint
{
    public static void MapBookEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/book");
        var linkGenerator = routes.ServiceProvider.GetRequiredService<LinkGenerator>();
        var getBookByIdUrl = linkGenerator.GetPathByName("GetBookById");

        group.MapGet("/", GetAllBooks)
            .WithName("GetAllBooks");

        group.MapGet("/{id}", GetBookById)
            .WithName("GetBookById");

        group.MapPut("/{id}", UpdateBook)
            .WithName("UpdateBook");

        group.MapPost("/", async Task<Created<Book>> (CreateBookRequest request, [FromServices] IBookManager manager) =>
            {
                var entity = await manager.CreateAsync(request.ToEntity());
                return TypedResults.Created($"{getBookByIdUrl}/{entity.BookId}", entity.ToView());
            })
            .WithName("CreateBook");

        group.MapDelete("/{id:guid}", DeleteBook)
            .WithName("DeleteBook");
    }

    private static async Task<Ok<PaginatedItems<Book>>> GetAllBooks([FromQuery] BookQueryFilter filter,
        [FromServices] IBookQueries query)
    {
        var books = await query.GetAllBooksAsync(filter);
        return TypedResults.Ok(books);
    }

    private static async Task<Results<Ok<Book>, NotFound>> GetBookById(Guid id, [FromServices] IBookQueries query)
    {
        var book = await query.GetBookAsync(id);
        return book is null ? TypedResults.NotFound() : TypedResults.Ok(book);
    }

    private static async Task<Results<Ok, NotFound>> UpdateBook(Guid id,
        [FromBody] UpdateBookRequest request,
        [FromServices] ScribeContext db,
        [FromServices] IBookManager manager)
    {
        var entity = await db.Books.FindAsync(id);
        if (entity is null)
        {
            return TypedResults.NotFound();
        }

        await manager.UpdateAsync(entity, request.ToEntity());
        return TypedResults.Ok();
    }

    private static async Task<Results<Ok, NotFound>> DeleteBook(
        Guid id,
        [FromServices] ScribeContext db,
        [FromServices] IBookManager manager)
    {
        var entity = await db.Books.FindAsync(id);
        if (entity is null)
        {
            return TypedResults.NotFound();
        }

        await manager.SetBookArchivedAsync(entity);
        return TypedResults.Ok();
    }
}