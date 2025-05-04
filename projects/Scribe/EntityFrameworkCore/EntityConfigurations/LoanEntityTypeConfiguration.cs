using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Scribe.EntityFrameworkCore.Stores;

namespace Scribe.EntityFrameworkCore.EntityConfigurations;

public class LoanEntityTypeConfiguration : IEntityTypeConfiguration<Loan>
{
    public void Configure(EntityTypeBuilder<Loan> builder)
    {
        builder.HasKey(l => l.LoanId);

        builder.Property(l => l.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(l => l.SubmissionDate)
            .IsRequired();

        builder.Property(l => l.ProcessingDate)
            .IsRequired(false);

        builder.HasOne<ScribeUser>()
            .WithMany()
            .HasForeignKey(l => l.BorrowerId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasOne<ScribeUser>()
            .WithMany()
            .HasForeignKey(l => l.ActorId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.HasMany(l => l.Books)
            .WithMany()
            .UsingEntity<LoanItem>();
    }
}