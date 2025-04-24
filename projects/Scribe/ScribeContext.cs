using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Scribe.AspNetCore.Identity.EntityFrameworkCore;
using Scribe.AspNetCore.Identity.Extensions.Stores;

// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength

namespace Scribe;

public class ScribeContext(DbContextOptions<ScribeContext> options)
    : IdentityUserContext<User, string, UserClaim<string>>(options)
{
    public DbSet<Book> Books { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<BorrowingRequestState> Requests { get; set; }
    public DbSet<Review> Reviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new BookEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new CategoryEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new BorrowingRequestEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ReviewEntityTypeConfiguration());
    }
}

public class Category
{
    public Guid Id { get; private init; }
    public required string Name { get; set; }
    public required string? Description { get; set; }
}

public class Book
{
    public Guid Id { get; private init; }
    public required string Title { get; set; }
    public required string Author { get; set; }
    public int Count { get; set; }
    public int Used { get; }
    public Guid CategoryId { get; set; }
    public Category Category { get; private init; } = null!;
}

public enum BorrowingRequestStatus
{
    Pending = 0,
    Accepted = 1,
    Rejected = 2
}

public class BorrowingRequestState
{
    public Guid Id { get; private init; }
    public Guid BorrowerId { get; private set; }
    public User Borrower { get; private init; } = null!;
    public Guid? ApproverId { get; private init; }
    public User? Approver { get; set; }
    public BorrowingRequestStatus Status { get; set; } = BorrowingRequestStatus.Pending;
    public DateTimeOffset DateRequested { get; set; }
    public DateTimeOffset? DateProcessed { get; set; }
}

public class Review
{
    public Guid Id { get; private init; }
    public Guid BookId { get; init; }
    public Guid UserId { get; init; }
    public bool Recommended { get; set; }
    public string? Comment { get; set; }
    public DateTimeOffset ReviewDate { get; set; }
}

public class CategoryEntityTypeConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).IsRequired().HasMaxLength(50);
        builder.HasIndex(c => c.Name).IsUnique();

        builder.Property(c => c.Description).HasMaxLength(500).IsRequired(false);

        builder.HasMany<Book>()
            .WithOne(b => b.Category)
            .HasForeignKey(b => b.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class BookEntityTypeConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(b => b.Author)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(b => b.Count)
            .IsRequired();

        builder.Property(b => b.Used)
            .IsRequired();
    }
}

public class BorrowingRequestEntityTypeConfiguration : IEntityTypeConfiguration<BorrowingRequestState>
{
    public void Configure(EntityTypeBuilder<BorrowingRequestState> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(b => b.DateRequested)
            .IsRequired();

        builder.Property(b => b.DateProcessed)
            .IsRequired(false);

        builder.HasOne(b => b.Borrower)
            .WithMany()
            .HasForeignKey(b => b.BorrowerId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasOne(b => b.Approver)
            .WithMany()
            .HasForeignKey(b => b.BorrowerId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        // TODO review navigation, yeah its really wrong....... gimme table splitting.....
    }
}

public class ReviewEntityTypeConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.BookId)
            .IsRequired();

        builder.Property(r => r.UserId)
            .IsRequired();

        builder.Property(r => r.Recommended)
            .IsRequired();

        builder.Property(r => r.Comment)
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(r => r.ReviewDate)
            .IsRequired();

        builder.HasIndex(r => new { r.BookId, r.UserId }).IsUnique();
    }
}