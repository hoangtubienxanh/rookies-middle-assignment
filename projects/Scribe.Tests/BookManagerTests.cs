using Api.Models.Book;
using Api.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Scribe.EntityFrameworkCore;
using Scribe.EntityFrameworkCore.Stores;

namespace Scribe.Tests;

[TestFixture]
public class BookManagerTests
{
    private ScribeContext _dbContext;
    private BookManager _bookManager;
    private TimeProvider _timeProvider;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<ScribeContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new ScribeContext(options);
        _timeProvider = TimeProvider.System;
        _bookManager = new BookManager(_timeProvider, _dbContext);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }

    [Test]
    public async Task CreateAsync_ShouldCreateBook_WhenValidOptionsProvided()
    {
        // Arrange
        var options = new BookCreateOptions
        {
            Title = "Test Book",
            Author = "Test Author",
            InputQuantity = 5,
            CategoryId = Guid.NewGuid()
        };

        // Act
        var book = await _bookManager.CreateAsync(options);

        // Assert
        Assert.That(book, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(book.Title, Is.EqualTo(options.Title));
            Assert.That(book.Author, Is.EqualTo(options.Author));
            Assert.That(book.Quantity, Is.EqualTo(options.InputQuantity));
            Assert.That(book.CategoryId, Is.EqualTo(options.CategoryId));
            Assert.That(book.LendingQuantity, Is.EqualTo(0));
        });
    }

    [Test]
    public async Task UpdateAsync_ShouldUpdateBook_WhenValidOptionsProvided()
    {
        // Arrange
        var book = new Book
        {
            Title = "Original Title",
            Author = "Original Author",
            Quantity = 10
        };
        _dbContext.Books.Add(book);
        await _dbContext.SaveChangesAsync();

        var options = new BookUpdateOptions
        {
            Title = "Updated Title",
            Author = "Updated Author",
            InputQuantity = 15,
            CategoryId = Guid.NewGuid()
        };

        // Act
        var updatedBook = await _bookManager.UpdateAsync(book, options);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(updatedBook.Title, Is.EqualTo(options.Title));
            Assert.That(updatedBook.Author, Is.EqualTo(options.Author));
            Assert.That(updatedBook.Quantity, Is.EqualTo(options.InputQuantity));
            Assert.That(updatedBook.CategoryId, Is.EqualTo(options.CategoryId));
        });
    }

    [Test]
    public async Task UpdateAsync_ShouldThrowException_WhenQuantityLessThanLendingQuantity()
    {
        // Arrange
        var book = new Book
        {
            Title = "Test Book",
            Author = "Test Author",
            Quantity = 10
        };
        _dbContext.Books.Add(book);

        var loan = new Loan
        {
            BookId = book.BookId,
            ApplicantId = Guid.NewGuid(),
            LoanApplicationId = Guid.NewGuid(),
            LoanDate = _timeProvider.GetUtcNow(),
            DueDate = _timeProvider.GetUtcNow().AddDays(14)
        };
        _dbContext.Loans.Add(loan);
        await _dbContext.SaveChangesAsync();

        var options = new BookUpdateOptions
        {
            Title = "Updated Title",
            Author = "Updated Author",
            InputQuantity = 0,
            CategoryId = null
        };

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _bookManager.UpdateAsync(book, options));
    }

    [Test]
    public async Task DeleteAsync_ShouldDeleteBook_WhenNoActiveLoans()
    {
        // Arrange
        var book = new Book
        {
            Title = "Test Book",
            Author = "Test Author",
            Quantity = 5
        };
        _dbContext.Books.Add(book);
        await _dbContext.SaveChangesAsync();

        // Act
        await _bookManager.DeleteAsync(book);

        // Assert
        var deletedBook = await _dbContext.Books.FindAsync(book.BookId);
        Assert.That(deletedBook, Is.Null);
    }

    [Test]
    public async Task GetAllBooksAsync_ShouldReturnPaginatedBooks()
    {
        // Arrange
        var books = Enumerable.Range(1, 15).Select(i => new Book
        {
            Title = $"Book {i}",
            Author = $"Author {i}",
            Quantity = i
        }).ToList();
        await _dbContext.Books.AddRangeAsync(books);
        await _dbContext.SaveChangesAsync();

        var options = new BookListOptions
        {
            PageIndex = 0,
            PageSize = 10
        };

        // Act
        var result = await _bookManager.GetAllBooksAsync(options);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.PageIndex, Is.EqualTo(0));
            Assert.That(result.PageSize, Is.EqualTo(10));
        });
    }

    [Test]
    public async Task GetBookAsync_ShouldReturnBook_WhenExists()
    {
        // Arrange
        var book = new Book
        {
            Title = "Test Book",
            Author = "Test Author",
            Quantity = 5
        };
        _dbContext.Books.Add(book);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _bookManager.GetBookAsync(book.BookId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Title, Is.EqualTo(book.Title));
            Assert.That(result.Author, Is.EqualTo(book.Author));
            Assert.That(result.Id, Is.EqualTo(book.BookId));
        });
    }

    [Test]
    public async Task GetBookAsync_ShouldReturnNull_WhenNotExists()
    {
        // Act
        var result = await _bookManager.GetBookAsync(Guid.NewGuid());

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task IncludeLendingQuantity_ShouldSetZero_WhenNoActiveLoans()
    {
        // Arrange
        var book = new Book
        {
            Title = "Test Book",
            Author = "Test Author",
            Quantity = 5
        };
        _dbContext.Books.Add(book);
        await _dbContext.SaveChangesAsync();

        // Act
        await _bookManager.IncludeLendingQuantity(book);

        // Assert
        Assert.That(book.LendingQuantity, Is.EqualTo(0));
    }

    [Test]
    public async Task IncludeLendingQuantity_ShouldCountActiveLoans()
    {
        // Arrange
        var book = new Book
        {
            Title = "Test Book",
            Author = "Test Author",
            Quantity = 5
        };
        _dbContext.Books.Add(book);

        var loans = Enumerable.Range(1, 3).Select(_ => new Loan
        {
            BookId = book.BookId,
            ApplicantId = Guid.NewGuid(),
            LoanApplicationId = Guid.NewGuid(),
            LoanDate = _timeProvider.GetUtcNow(),
            DueDate = _timeProvider.GetUtcNow().AddDays(14)
        }).ToList();
        
        await _dbContext.Loans.AddRangeAsync(loans);
        await _dbContext.SaveChangesAsync();

        // Act
        await _bookManager.IncludeLendingQuantity(book);

        // Assert
        Assert.That(book.LendingQuantity, Is.EqualTo(3));
    }

    [Test]
    public async Task IncludeLendingQuantity_ShouldCountActiveLoansForMultipleBooks()
    {
        // Arrange
        var books = Enumerable.Range(1, 3).Select(i => new Book
        {
            Title = $"Book {i}",
            Author = $"Author {i}",
            Quantity = 5
        }).ToList();
        await _dbContext.Books.AddRangeAsync(books);

        // Add 2 loans for first book, 1 for second, none for third
        var loans = new List<Loan>
        {
            new()
            {
                BookId = books[0].BookId,
                ApplicantId = Guid.NewGuid(),
                LoanApplicationId = Guid.NewGuid(),
                LoanDate = _timeProvider.GetUtcNow(),
                DueDate = _timeProvider.GetUtcNow().AddDays(14)
            },
            new()
            {
                BookId = books[0].BookId,
                ApplicantId = Guid.NewGuid(),
                LoanApplicationId = Guid.NewGuid(),
                LoanDate = _timeProvider.GetUtcNow(),
                DueDate = _timeProvider.GetUtcNow().AddDays(14)
            },
            new()
            {
                BookId = books[1].BookId,
                ApplicantId = Guid.NewGuid(),
                LoanApplicationId = Guid.NewGuid(),
                LoanDate = _timeProvider.GetUtcNow(),
                DueDate = _timeProvider.GetUtcNow().AddDays(14)
            }
        };
        
        await _dbContext.Loans.AddRangeAsync(loans);
        await _dbContext.SaveChangesAsync();

        // Act
        await _bookManager.IncludeLendingQuantity(books);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(books[0].LendingQuantity, Is.EqualTo(2));
            Assert.That(books[1].LendingQuantity, Is.EqualTo(1));
            Assert.That(books[2].LendingQuantity, Is.EqualTo(0));
        });
    }

    [Test]
    public async Task DeleteAsync_ShouldThrowException_WhenBookHasActiveLoans()
    {
        // Arrange
        var book = new Book
        {
            Title = "Test Book",
            Author = "Test Author",
            Quantity = 5
        };
        _dbContext.Books.Add(book);

        var loan = new Loan
        {
            BookId = book.BookId,
            ApplicantId = Guid.NewGuid(),
            LoanApplicationId = Guid.NewGuid(),
            LoanDate = _timeProvider.GetUtcNow(),
            DueDate = _timeProvider.GetUtcNow().AddDays(14)
        };
        _dbContext.Loans.Add(loan);
        await _dbContext.SaveChangesAsync();

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _bookManager.DeleteAsync(book));
    }
}