using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Scribe.EntityFrameworkCore.Stores;
using Scribe.EntityFrameworkCore.ValueConverters;

namespace Scribe.EntityFrameworkCore.EntityConfigurations;

public class LoanEntityTypeConfiguration : IEntityTypeConfiguration<Loan>
{
    public void Configure(EntityTypeBuilder<Loan> builder)
    {
        builder.HasKey(l => l.LoanId);

        builder.HasOne(l => l.Book)
            .WithMany()
            .HasForeignKey(l => l.BookId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(l => l.Applicant)
            .WithMany(u => u.Loans)
            .HasForeignKey(l => l.ApplicantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(l => l.LoanApplication)
            .WithMany(u => u.LendingItems)
            .HasForeignKey(l => l.LoanApplicationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(l => l.LoanDate)
            .HasConversion(new DateTimeOffsetToUtcDateTimeTicksConverter());

        builder.Property(l => l.DueDate)
            .HasConversion(new DateTimeOffsetToUtcDateTimeTicksConverter());

        builder.Property(l => l.ReturnDate)
            .HasConversion(new DateTimeOffsetToUtcDateTimeTicksConverter());
    }
}