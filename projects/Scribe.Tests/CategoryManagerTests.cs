using Api.Models;
using Api.Models.Category;
using Api.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Scribe.EntityFrameworkCore;
using Scribe.EntityFrameworkCore.Stores;

namespace Scribe.Tests;

[TestFixture]
public class CategoryManagerTests
{
    private ScribeContext _dbContext;
    private CategoryManager _categoryManager;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<ScribeContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new ScribeContext(options);
        _categoryManager = new CategoryManager(_dbContext);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }

    [Test]
    public async Task CreateAsync_ShouldCreateCategory_WhenValidOptionsProvided()
    {
        // Arrange
        var options = new CategoryCreateOptions
        {
            Name = "Test Category"
        };

        // Act
        var category = await _categoryManager.CreateAsync(options);

        // Assert
        Assert.That(category, Is.Not.Null);
        Assert.That(category.Name, Is.EqualTo(options.Name));
        Assert.That(category.CategoryId, Is.Not.EqualTo(Guid.Empty));
    }

    [Test]
    public async Task UpdateAsync_ShouldUpdateCategory_WhenValidOptionsProvided()
    {
        // Arrange
        var category = new Category { Name = "Original Name" };
        _dbContext.Categories.Add(category);
        await _dbContext.SaveChangesAsync();

        var options = new CategoryUpdateOptions
        {
            Name = "Updated Name"
        };

        // Act
        var updatedCategory = await _categoryManager.UpdateAsync(category, options);

        // Assert
        Assert.That(updatedCategory.Name, Is.EqualTo(options.Name));
    }

    [Test]
    public async Task DeleteAsync_ShouldDeleteCategory_WhenNotInUse()
    {
        // Arrange
        var category = new Category { Name = "Test Category" };
        _dbContext.Categories.Add(category);
        await _dbContext.SaveChangesAsync();

        // Act
        await _categoryManager.DeleteAsync(category);

        // Assert
        var deletedCategory = await _dbContext.Categories.FindAsync(category.CategoryId);
        Assert.That(deletedCategory, Is.Null);
    }
    
    [Test]
    public async Task GetByIdAsync_ShouldReturnCategory_WhenExists()
    {
        // Arrange
        var category = new Category { Name = "Test Category" };
        _dbContext.Categories.Add(category);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _categoryManager.GetByIdAsync(category.CategoryId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo(category.Name));
        Assert.That(result.CategoryId, Is.EqualTo(category.CategoryId));
    }

    [Test]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        // Act
        var result = await _categoryManager.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetAllAsync_ShouldReturnPaginatedCategories()
    {
        // Arrange
        var categories = Enumerable.Range(1, 15)
            .Select(i => new Category { Name = $"Category {i}" })
            .ToList();
        await _dbContext.Categories.AddRangeAsync(categories);
        await _dbContext.SaveChangesAsync();

        var options = new CategoryListOptions
        {
            PageIndex = 0,
            PageSize = 10
        };

        // Act
        var result = await _categoryManager.GetAllAsync(options);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.PageIndex, Is.EqualTo(0));
            Assert.That(result.PageSize, Is.EqualTo(10));
        });
    }
}