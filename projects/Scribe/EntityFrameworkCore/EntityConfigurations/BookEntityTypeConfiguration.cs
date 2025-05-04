using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Scribe.EntityFrameworkCore.Stores;

namespace Scribe.EntityFrameworkCore.EntityConfigurations;

public class BookEntityTypeConfiguration : IEntityTypeConfiguration<Book>
{
    // private const string BorrowCountComputedCommandText = "";

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

        builder.Ignore(b => b.BorrowedQuantity);
        // .HasComputedColumnSql(BorrowCountComputedCommandText, stored: false);

        builder
            .HasOne(e => e.Category)
            .WithMany()
            .HasForeignKey(e => e.CategoryId)
            .IsRequired(false);
    }
}