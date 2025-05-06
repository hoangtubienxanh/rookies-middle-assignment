using Api.Models;
using Api.Models.Category;

using Microsoft.EntityFrameworkCore;

using Scribe.EntityFrameworkCore;
using Scribe.EntityFrameworkCore.Stores;

namespace Api.Services;

public class CategoryManager(ScribeContext context) : ICategoryManager
{
    public async Task<Category> CreateAsync(CategoryCreateOptions options)
    {
        var category = new Category { Name = options.Name };

        context.Categories.Add(category);
        await context.SaveChangesAsync();
        return category;
    }

    public async Task<Category> UpdateAsync(Category category, CategoryUpdateOptions options)
    {
        category.Name = options.Name;
        await context.SaveChangesAsync();
        return category;
    }

    public async Task DeleteAsync(Category category)
    {
        try
        {
            context.Categories.Remove(category);
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException("Cannot delete category that is currently in use.", ex);
        }
    }

    public async Task<Category?> GetByIdAsync(Guid categoryId)
    {
        return await context.Categories.FindAsync(categoryId);
    }

    public async Task<PaginatedItems<Category>> GetAllAsync(CategoryListOptions options)
    {
        var totalCategories = await context.Categories.CountAsync();

        var categories = await context.Categories
            .OrderBy(c => c.CategoryId)
            .Skip(options.PageIndex * options.PageSize)
            .Take(options.PageSize)
            .ToListAsync();

        return new PaginatedItems<Category>(options.PageIndex, options.PageSize, totalCategories, categories);
    }
}