using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Scribe.EntityFrameworkCore.Stores;

namespace Scribe.EntityFrameworkCore.EntityConfigurations;

public class ApplicationItemEntityTypeConfiguration : IEntityTypeConfiguration<ApplicationItem>
{
    public void Configure(EntityTypeBuilder<ApplicationItem> builder)
    {
        builder.HasKey(ai => ai.ApplicationItemId);

        builder.HasOne(ai => ai.Book)
            .WithMany()
            .HasForeignKey(ai => ai.BookId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}