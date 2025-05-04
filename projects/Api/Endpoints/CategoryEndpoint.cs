using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

using Scribe.EntityFrameworkCore;
using Scribe.EntityFrameworkCore.Stores;

namespace Api.Endpoints;

public static class CategoryEndpoint
{
    public static void MapCategoryEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/category");
        group.RequireAuthorization("all_access");

        group.MapGet("/", GetAllCategories)
            .WithName("GetAllCategories");

        group.MapGet("/{id}", GetCategoryById)
            .WithName("GetCategoryById");

        group.MapPut("/{id}", UpdateCategory)
            .WithName("UpdateCategory");

        group.MapPost("/", CreateCategory)
            .WithName("CreateCategory");

        group.MapDelete("/{id}", DeleteCategory)
            .WithName("DeleteCategory");
    }

    private static async Task<Results<Ok, NotFound>> DeleteCategory(Guid categoryid, ScribeContext db)
    {
        var affected = await db.Categories.Where(model => model.CategoryId == categoryid)
            .ExecuteDeleteAsync();
        return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
    }


    private static async Task<List<Category>> GetAllCategories(ScribeContext db)
    {
        return await db.Categories.ToListAsync();
    }

    private static async Task<Results<Ok<Category>, NotFound>> GetCategoryById(Guid categoryid, ScribeContext db)
    {
        return await db.Categories.AsNoTracking()
            .FirstOrDefaultAsync(model => model.CategoryId == categoryid) is Category model
            ? TypedResults.Ok(model)
            : TypedResults.NotFound();
    }

    private static async Task<Results<Ok, NotFound>> UpdateCategory(Guid categoryid, Category category,
        ScribeContext db)
    {
        var affected = await db.Categories.Where(model => model.CategoryId == categoryid)
            .ExecuteUpdateAsync(setters => setters.SetProperty(m => m.CategoryId, category.CategoryId)
                .SetProperty(m => m.Name, category.Name)
                .SetProperty(m => m.Slug, category.Slug));
        return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
    }

    private static async Task<Created<Category>> CreateCategory(Category category, ScribeContext db)
    {
        db.Categories.Add(category);
        await db.SaveChangesAsync();
        return TypedResults.Created($"/api/Category/{category.CategoryId}", category);
    }
}