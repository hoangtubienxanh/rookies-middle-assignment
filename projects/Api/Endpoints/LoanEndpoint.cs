using Api.Services;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Scribe.EntityFrameworkCore;
using Scribe.Models;
using Scribe.Services;

namespace Api.Endpoints;

public static class LoanEndpoint
{
    public static RouteGroupBuilder MapLoanApi(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/loan")
            .RequireAuthorization();

        var linkGenerator = routes.ServiceProvider.GetRequiredService<LinkGenerator>();
        var getLoanById = linkGenerator.GetPathByName("GetLoanById");

        group.MapGet("/{id:guid}", GetLoan)
            .WithName("GetLoanById");

        group.MapPost("/", async Task<Results<Created<Loan>, ValidationProblem>> (CreateLoanRequest request,
                [FromServices] CurrentUser me,
                [FromServices] ILoanManager manager,
                [FromServices] ScribeContext db) =>
            {
                Dictionary<string, string[]> errors = [];

                if (request.LoanItemOptions.Count == 0)
                {
                    errors.TryAdd("items", ["At least one book must be selected to loan."]);
                    return TypedResults.ValidationProblem(errors);
                }

                var books = await db.Books.Where(b => request.LoanItemOptions.Contains(b.BookId)).ToListAsync();

                if (books.Count != request.LoanItemOptions.Count)
                {
                    errors.TryAdd("items", ["One or more books are not available to loan."]);
                    return TypedResults.ValidationProblem(errors);
                }

                var entity = await manager.CreateAsync(me.Id, request.ToEntity());
                await manager.AddLoanItemsAsync(entity, books);
                return TypedResults.Created($"{getLoanById}/{entity.LoanId}", entity.ToView());
            })
            .WithName("CreateLoan");

        group.MapPost("/{id:guid}/item/{item:guid}/extend", ExtendLoanItem);

        var managementGroup = group.MapGroup("/authorized/loan")
            .RequireAuthorization("all_access");

        return group;
    }

    private static async Task<Results<Ok<Loan>, BadRequest>> ExtendLoanItem(Guid id, Guid itemId)
    {
        throw new NotImplementedException();
    }

    private static async Task<Results<Ok<Loan>, ValidationProblem>> GetLoan(Guid id)
    {
        throw new NotImplementedException();
    }
}