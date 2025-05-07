using Api.Models;
using Api.Models.Loan;

using Microsoft.EntityFrameworkCore;

using Scribe.EntityFrameworkCore;
using Scribe.EntityFrameworkCore.Stores;

namespace Api.Services;

public class LoanApplicationManager(TimeProvider timeProvider, ScribeContext context, IBookManager bookManager)
    : ILoanApplicationManager
{
    private const int MaxItemsPerApplication = 5;
    private const int MaxProcessingApplicationPerMonth = 3;

    public async Task<LoanApplication> CreateAsync(Guid applicantId, params List<Book> books)
    {
        if (books.Count > MaxItemsPerApplication)
        {
            throw new InvalidOperationException("You can only apply for up to 5 books at a time.");
        }

        var now = timeProvider.GetUtcNow();
        var collection = await context.LoanApplications
            .Where(la => la.ApplicantId == applicantId)
            .Where(la => la.Status == LoanStatus.Approved || la.Status == LoanStatus.Open)
            .ToListAsync();

        // TODO: client evaluated fix due to sqlite provider not handling datetimeoffset comparasion
        var approvedApplicationsThisMonth = collection
            .Count(la => la.ApplicationDate.Year == now.Year && la.ApplicationDate.Month == now.Month);

        if (approvedApplicationsThisMonth >= MaxProcessingApplicationPerMonth)
        {
            throw new InvalidOperationException(
                "You can not apply for new applications at the moment. Please try again later.");
        }

        var loanApplication = new LoanApplication
        {
            ApplicantId = applicantId, ApplicationDate = timeProvider.GetUtcNow()
        };

        loanApplication.ApplicationItems.AddRange(books);

        await context.LoanApplications.AddAsync(loanApplication);
        await context.SaveChangesAsync();
        return loanApplication;
    }

    public async Task<LoanApplication> ApproveAsync(Guid actorId, LoanApplication loanApplication)
    {
        if (loanApplication.Status != LoanStatus.Open)
        {
            throw new InvalidOperationException("Loan application is not open.");
        }

        await bookManager.IncludeLendingQuantity(loanApplication.ApplicationItems);
        if (loanApplication.ApplicationItems.Any(book => book.AvailableQuantity() <= 0))
        {
            throw new InvalidOperationException("One or more items is no longer available.");
        }

        loanApplication.ActorId = actorId;
        loanApplication.DecisionDate = timeProvider.GetUtcNow();
        loanApplication.Status = LoanStatus.Approved;

        var loans = loanApplication.ApplicationItems.Select(item => new Loan
        {
            BookId = item.BookId,
            ApplicantId = loanApplication.ApplicantId,
            LoanApplicationId = loanApplication.LoanApplicationId,
            LoanDate = timeProvider.GetUtcNow(),
            DueDate = timeProvider.GetUtcNow().AddDays(14)
        }).ToList();

        await context.Loans.AddRangeAsync(loans);

        await context.SaveChangesAsync();
        return loanApplication;
    }

    public async Task<LoanApplication> DenyAsync(Guid actorId, LoanApplication loanApplication)
    {
        if (loanApplication.Status != LoanStatus.Open)
        {
            throw new InvalidOperationException("Loan application is not open.");
        }

        loanApplication.ActorId = actorId;
        loanApplication.DecisionDate = timeProvider.GetUtcNow();
        loanApplication.Status = LoanStatus.Denied;

        await context.SaveChangesAsync();
        return loanApplication;
    }

    public async Task<PaginatedItems<LoanApplicationItem>> GetAllAsync(LoanApplicationListOptions options)
    {
        var totalLoans = await context.LoanApplications.CountAsync();

        var loanApplications = await context.LoanApplications
            .OrderBy(la => la.LoanApplicationId)
            .Skip(options.PageIndex * options.PageSize)
            .Take(options.PageSize)
            .Select(la => la.AsLoanApplicationItem())
            .ToListAsync();

        return new PaginatedItems<LoanApplicationItem>(options.PageIndex, options.PageSize, totalLoans,
            loanApplications);
    }

    public async Task<PaginatedItems<LoanApplicationItem>> GetAllByApplicantAsync(Guid applicantId, LoanStatus status,
        LoanApplicationListOptions options)
    {
        var query = context.LoanApplications
            .Where(la => la.ApplicantId == applicantId && la.Status == status);

        var totalItems = await query.CountAsync();

        var loanApplications = await query
            .OrderBy(la => la.LoanApplicationId)
            .Skip(options.PageIndex * options.PageSize)
            .Take(options.PageSize)
            .Select(la => la.AsLoanApplicationItem())
            .ToListAsync();

        return new PaginatedItems<LoanApplicationItem>(options.PageIndex, options.PageSize, totalItems,
            loanApplications);
    }
}