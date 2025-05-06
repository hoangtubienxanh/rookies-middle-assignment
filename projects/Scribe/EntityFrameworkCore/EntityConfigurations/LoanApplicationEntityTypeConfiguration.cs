using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Scribe.EntityFrameworkCore.Stores;
using Scribe.EntityFrameworkCore.ValueConverters;

namespace Scribe.EntityFrameworkCore.EntityConfigurations;

public class LoanApplicationEntityTypeConfiguration : IEntityTypeConfiguration<LoanApplication>
{
    public void Configure(EntityTypeBuilder<LoanApplication> builder)
    {
        builder.HasKey(la => la.LoanApplicationId);

        builder.Property(la => la.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(la => la.ApplicationDate)
            .IsRequired();

        builder.Property(la => la.DecisionDate)
            .IsRequired(false);

        builder.HasOne<ScribeUser>()
            .WithMany()
            .HasForeignKey(la => la.ApplicantId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasOne<ScribeUser>()
            .WithMany()
            .HasForeignKey(la => la.ActorId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.HasMany(la => la.ApplicationItems)
            .WithMany()
            .UsingEntity<ApplicationItem>();

        builder.Navigation(la => la.ApplicationItems).AutoInclude();

        builder.Property(la => la.ApplicationDate)
            .HasConversion(new DateTimeOffsetToUtcDateTimeTicksConverter());

        builder.Property(la => la.DecisionDate)
            .HasConversion(new DateTimeOffsetToUtcDateTimeTicksConverter());
    }
}