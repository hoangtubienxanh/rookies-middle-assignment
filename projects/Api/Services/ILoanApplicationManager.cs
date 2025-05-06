using Api.Models;
using Api.Models.Loan;

using Scribe.EntityFrameworkCore.Stores;

namespace Api.Services;

public interface ILoanApplicationManager
{
    public Task<LoanApplication> CreateAsync(Guid applicantId, params List<Book> books);
    public Task<LoanApplication> ApproveAsync(Guid actorId, LoanApplication loanApplication);
    public Task<LoanApplication> DenyAsync(Guid actorId, LoanApplication loanApplication);
    public Task<PaginatedItems<LoanApplicationItem>> GetAllAsync(LoanApplicationListOptions options);
    public Task<PaginatedItems<LoanApplicationItem>> GetAllByApplicantAsync(Guid applicantId, LoanStatus status, LoanApplicationListOptions options);
}