using Api.Models;
using Api.Models.Category;

using Scribe.EntityFrameworkCore.Stores;

namespace Api.Services;

public interface ICategoryManager
{
    Task<Category> CreateAsync(CategoryCreateOptions options);
    Task<Category> UpdateAsync(Category category, CategoryUpdateOptions options);
    Task DeleteAsync(Category category);
    Task<Category?> GetByIdAsync(Guid categoryId);
    Task<PaginatedItems<Category>> GetAllAsync(CategoryListOptions options);
}