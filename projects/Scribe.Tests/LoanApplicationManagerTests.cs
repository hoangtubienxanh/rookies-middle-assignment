using Api.Services;

using Microsoft.EntityFrameworkCore;

using Moq;

using Scribe.EntityFrameworkCore;
using Scribe.EntityFrameworkCore.Stores;

namespace Scribe.Tests;

[TestFixture]
public class LoanApplicationManagerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<ScribeContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new ScribeContext(options);
        _bookManagerMock = new Mock<IBookManager>();
        _loanApplicationManager = new LoanApplicationManager(
            TimeProvider.System,
            _dbContext,
            _bookManagerMock.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }

    private ScribeContext _dbContext;
    private Mock<IBookManager> _bookManagerMock;
    private LoanApplicationManager _loanApplicationManager;

    [Test]
    public async Task CreateAsync_ShouldCreateLoanApplication_WhenValidBooksProvided()
    {
        // Arrange
        var applicantId = Guid.NewGuid();
        var books = new List<Book>
        {
            new() { Title = "Book 1", Author = "Author 1", Quantity = 5 },
            new() { Title = "Book 2", Author = "Author 2", Quantity = 3 }
        };

        _dbContext.Books.AddRange(books);
        await _dbContext.SaveChangesAsync();

        // Act
        var loanApplication = await _loanApplicationManager.CreateAsync(applicantId, books);

        // Assert
        Assert.That(loanApplication, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(loanApplication.ApplicantId, Is.EqualTo(applicantId));
            Assert.That(loanApplication.ApplicationItems, Has.Count.EqualTo(2));
        });
    }

    [Test]
    public void CreateAsync_ShouldThrowException_WhenTooManyBooksProvided()
    {
        // Arrange
        var applicantId = Guid.NewGuid();
        var books = Enumerable.Range(1, 6).Select(i => new Book
        {
            Title = $"Book {i}", Author = $"Author {i}", Quantity = 5
        }).ToList();

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _loanApplicationManager.CreateAsync(applicantId, books));
    }

    [Test]
    public async Task ApproveAsync_ShouldApproveLoanApplication_WhenValid()
    {
        // Arrange
        var applicantId = Guid.NewGuid();
        var actorId = Guid.NewGuid();
        var books = new List<Book>
        {
            new() { Title = "Book 1", Author = "Author 1", Quantity = 5, LendingQuantity = 0 }
        };

        var loanApplication = new LoanApplication { ApplicantId = applicantId, Status = LoanStatus.Open };
        loanApplication.ApplicationItems.AddRange(books);

        _dbContext.LoanApplications.Add(loanApplication);
        await _dbContext.SaveChangesAsync();

        _bookManagerMock
            .Setup(m => m.IncludeLendingQuantity(It.IsAny<List<Book>>()))
            .Returns(Task.CompletedTask);

        // Act
        var approvedApplication = await _loanApplicationManager.ApproveAsync(actorId, loanApplication);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(approvedApplication.Status, Is.EqualTo(LoanStatus.Approved));
            Assert.That(approvedApplication.ActorId, Is.EqualTo(actorId));
        });
    }
}