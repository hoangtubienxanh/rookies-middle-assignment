using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Scribe.EntityFrameworkCore.Stores;

namespace Scribe.EntityFrameworkCore.EntityConfigurations;

public class CategoryEntityTypeConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(c => c.CategoryId);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(-1);

        builder.HasIndex(c => c.Name)
            .IsUnique();

        builder.Property(c => c.Slug)
            .IsRequired(false)
            .HasMaxLength(-1);

        builder.HasMany<Book>()
            .WithOne(b => b.Category)
            .HasForeignKey(b => b.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}