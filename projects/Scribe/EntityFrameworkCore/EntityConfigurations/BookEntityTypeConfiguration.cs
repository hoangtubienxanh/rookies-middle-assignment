using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Scribe.EntityFrameworkCore.Stores;

namespace Scribe.EntityFrameworkCore.EntityConfigurations;

public class BookEntityTypeConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.HasKey(b => b.BookId);

        builder.Property(b => b.Title)
            .IsRequired()
            .HasMaxLength(-1);

        builder.Property(b => b.Author)
            .IsRequired()
            .HasMaxLength(-1);

        builder.Property(b => b.Quantity)
            .IsRequired();

        builder.HasOne(b => b.Category)
            .WithMany()
            .HasForeignKey(b => b.CategoryId)
            .IsRequired(false);

        builder.Ignore(b => b.LendingQuantity);
    }
}