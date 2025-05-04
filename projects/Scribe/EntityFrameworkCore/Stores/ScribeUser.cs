using Microsoft.AspNetCore.Identity;

namespace Scribe.EntityFrameworkCore.Stores;

public class ScribeUser : IdentityUser<Guid>
{
    public List<Loan> Loans { get; } = [];
    public List<Review> Reviews { get; } = [];
}