using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Scribe.EntityFrameworkCore.Stores;

namespace Scribe.EntityFrameworkCore.EntityConfigurations;

public class ScribeUserEntityTypeConfiguration : IEntityTypeConfiguration<ScribeUser>
{
    public void Configure(EntityTypeBuilder<ScribeUser> builder)
    {
        builder.HasBaseType<IdentityUser<Guid>>();

        builder.HasMany(u => u.Loans)
            .WithOne()
            .HasForeignKey("UserId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.Reviews)
            .WithOne()
            .HasForeignKey("UserId")
            .OnDelete(DeleteBehavior.Restrict);
    }
}