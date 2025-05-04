using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Scribe.EntityFrameworkCore.Stores;

namespace Scribe.EntityFrameworkCore.EntityConfigurations;

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