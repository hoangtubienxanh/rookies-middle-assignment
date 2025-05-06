using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using Scribe.EntityFrameworkCore.EntityConfigurations;
using Scribe.EntityFrameworkCore.Stores;

namespace Scribe.EntityFrameworkCore;

public class ScribeContext(DbContextOptions<ScribeContext> options)
    : IdentityUserContext<ScribeUser, Guid>(options)
{
    public DbSet<Book> Books { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<LoanApplication> LoanApplications { get; set; }
    public DbSet<Loan> Loans { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<ApplicationItem> ApplicationItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new BookEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new CategoryEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new LoanApplicationEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new LoanEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ApplicationItemEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ReviewEntityTypeConfiguration());
    }
}