namespace Scribe.Models;

public static class LoanViewModelExtensions
{
    public static EntityFrameworkCore.Stores.Loan ToEntity(this CreateLoanRequest newLoan)
    {
        var loan = new EntityFrameworkCore.Stores.Loan();
        return loan;
    }

    public static Loan ToView(this EntityFrameworkCore.Stores.Loan entity)
    {
        return new Loan();
    }
}