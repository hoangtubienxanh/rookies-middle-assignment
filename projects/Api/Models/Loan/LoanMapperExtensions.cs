using Api.Models.Book;

using Scribe.EntityFrameworkCore.Stores;

namespace Api.Models.Loan;

public static class LoanMapperExtensions
{
    public static LoanApplicationItem AsLoanApplicationItem(this LoanApplication loan)
    {
        return new LoanApplicationItem
        {
            Id = loan.LoanApplicationId,
            Created = loan.ApplicationDate,
            Status = loan.Status.ToString(),
            Items = loan.ApplicationItems.Select(book => book.AsBookItem()).ToList()
        };
    }
}