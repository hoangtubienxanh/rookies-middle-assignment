using Api.Models;
using Api.Models.Category;
using Api.Services;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using Scribe.EntityFrameworkCore.Stores;

namespace Api.Endpoints;

public static class CategoryEndpoint
{
    public static RouteGroupBuilder MapCategoryEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/category");

        var authorizedGroup = group.MapGroup("")
            .RequireAuthorization("all_access");

        group.MapGet("/", GetAllCategories)
            .WithName("GetAllCategories")
            .WithParameterValidation();

        group.MapGet("/{id:guid}", GetCategoryById)
            .WithName("GetCategoryById");

        authorizedGroup.MapPut("/{id:guid}", UpdateCategory)
            .WithName("UpdateCategory")
            .WithParameterValidation();

        authorizedGroup.MapPost("/", CreateCategory)
            .WithName("CreateCategory")
            .WithParameterValidation();

        authorizedGroup.MapDelete("/{id:guid}", DeleteCategory)
            .WithName("DeleteCategory");

        return group;
    }

    public static async Task<Ok<PaginatedItems<Category>>> GetAllCategories(
        [AsParameters] CategoryListOptions options,
        [FromServices] ICategoryManager categoryManager)
    {
        var categories = await categoryManager.GetAllAsync(options);
        return TypedResults.Ok(categories);
    }

    public static async Task<Results<Ok<Category>, NotFound<ProblemDetails>>> GetCategoryById(
        Guid id,
        ICategoryManager categoryManager)
    {
        var category = await categoryManager.GetByIdAsync(id);
        return category != null ? TypedResults.Ok(category) : CreateNotFoundProblemDetail(id);
    }

    public static async Task<IResult> UpdateCategory(
        Guid id,
        [AsParameters] CategoryUpdateOptions options,
        ICategoryManager categoryManager)
    {
        var category = await categoryManager.GetByIdAsync(id);
        if (category == null)
        {
            return CreateNotFoundProblemDetail(id);
        }

        await categoryManager.UpdateAsync(category, options);
        return TypedResults.Ok(category);
    }

    public static async Task<IResult> CreateCategory(
        [AsParameters] CategoryCreateOptions options,
        ICategoryManager categoryManager)
    {
        var createdCategory = await categoryManager.CreateAsync(options);
        return TypedResults.Ok(createdCategory);
    }

    public static async Task<IResult> DeleteCategory(
        Guid id,
        ICategoryManager categoryManager)
    {
        var category = await categoryManager.GetByIdAsync(id);
        if (category == null)
        {
            return CreateNotFoundProblemDetail(id);
        }

        await categoryManager.DeleteAsync(category);
        return TypedResults.Ok();
    }

    private static NotFound<ProblemDetails> CreateNotFoundProblemDetail(Guid id)
    {
        return TypedResults.NotFound(new ProblemDetails { Detail = $"Category with id {id} not found." });
    }
}