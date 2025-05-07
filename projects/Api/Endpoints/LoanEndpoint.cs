using Api.Models;
using Api.Models.Loan;
using Api.Services;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Scribe.EntityFrameworkCore;
using Scribe.EntityFrameworkCore.Stores;

namespace Api.Endpoints;

public static class LoanEndpoint
{
    public static RouteGroupBuilder MapLoanApi(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/loan")
            .RequireAuthorization();

        var authorizedGroup = routes.MapGroup("/loan")
            .RequireAuthorization("all_access");

        group.MapGet("/", GetAllLoanApplications)
            .WithName("GetAllLoanApplications");

        group.MapGet("/{id:guid}", GetLoanApplicationById)
            .RequireAuthorization("applicant_creator", "all_access")
            .WithName("GetLoanApplicationById");

        group.MapPost("/", CreateLoanApplication)
            .WithName("CreateLoanApplication");

        authorizedGroup.MapPut("/{id:guid}", UpdateLoanApplication)
            .WithName("UpdateLoanApplication");

        return group;
    }

    public static async Task<Results<Ok<LoanApplicationItem>, NotFound<ProblemDetails>>> GetLoanApplicationById(
        [FromRoute] Guid id,
        [FromServices] ScribeContext context)
    {
        var loanApplication = await context.LoanApplications.SingleOrDefaultAsync(la => la.LoanApplicationId == id);
        if (loanApplication is null)
        {
            return CreateNotFoundProblemDetail(id);
        }

        return TypedResults.Ok(loanApplication.AsLoanApplicationItem());
    }

    public static async Task<Results<Ok<LoanApplicationItem>, BadRequest<ProblemDetails>>> CreateLoanApplication(
        [AsParameters] LoanApplicationCreateOptions options,
        [FromServices] CurrentUser me,
        [FromServices] ScribeContext context,
        [FromServices] ILoanApplicationManager applicationManager)
    {
        var books = await context.Books.Where(b => options.Items.Contains(b.BookId)).ToListAsync();

        if (books.Count != options.ValidatedItems.Count)
        {
            TypedResults.BadRequest(new ProblemDetails { Detail = "One or more item was not found." });
        }

        var loanApplication = await applicationManager.CreateAsync(me.Id, books);
        return TypedResults.Ok(loanApplication.AsLoanApplicationItem());
    }

    public static async Task<Results<Ok<LoanApplicationItem>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>>
        UpdateLoanApplication(
            [AsParameters] LoanApplicationUpdateOptions options,
            [FromRoute] Guid id,
            [FromServices] CurrentUser me,
            [FromServices] ScribeContext context,
            [FromServices] ILoanApplicationManager applicationManager)
    {
        var loanApplication = await context.LoanApplications.SingleOrDefaultAsync(la => la.LoanApplicationId == id);
        if (loanApplication is null)
        {
            return CreateNotFoundProblemDetail(id);
        }

        _ = Enum.TryParse<LoanStatus>(options.Status, out var status);
        switch (status)
        {
            case LoanStatus.Approved:
                await applicationManager.ApproveAsync(me.Id, loanApplication);
                break;
            case LoanStatus.Denied:
                await applicationManager.DenyAsync(me.Id, loanApplication);
                break;
            case LoanStatus.Open:
            default:
                throw new ArgumentOutOfRangeException(nameof(status));
        }

        return TypedResults.Ok(loanApplication.AsLoanApplicationItem());
    }

    public static async Task<Ok<PaginatedItems<LoanApplicationItem>>> GetAllLoanApplications(
        [AsParameters] LoanApplicationListOptions options,
        [FromServices] ILoanApplicationManager loanManager)
    {
        var loans = await loanManager.GetAllAsync(options);
        return TypedResults.Ok(loans);
    }

    private static NotFound<ProblemDetails> CreateNotFoundProblemDetail(Guid id)
    {
        return TypedResults.NotFound(new ProblemDetails { Detail = $"Item with id {id} not found." });
    }
}