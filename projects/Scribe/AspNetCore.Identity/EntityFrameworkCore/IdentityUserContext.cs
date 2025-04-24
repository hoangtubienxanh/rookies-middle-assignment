// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Scribe.AspNetCore.Identity.Extensions.Stores;

namespace Scribe.AspNetCore.Identity.EntityFrameworkCore;

/// <summary>
///     Base class for the Entity Framework database context used for identity.
/// </summary>
/// <typeparam name="TUser">The type of user objects.</typeparam>
/// <typeparam name="TKey">The type of the primary key for users and roles.</typeparam>
/// <typeparam name="TUserClaim">The type of the user claim object.</typeparam>
[ExcludeFromCodeCoverage(Justification = "derived from Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserContext")]
public abstract class IdentityUserContext<TUser, TKey, TUserClaim> : DbContext
    where TUser : User<TKey>
    where TKey : IEquatable<TKey>
    where TUserClaim : UserClaim<TKey>
{
    /// <summary>
    ///     Initializes a new instance of the class.
    /// </summary>
    /// <param name="options">The options to be used by a <see cref="Microsoft.EntityFrameworkCore.DbContext" />.</param>
    protected IdentityUserContext(DbContextOptions options) : base(options) { }

    /// <summary>
    ///     Initializes a new instance of the class.
    /// </summary>
    protected IdentityUserContext() { }

    /// <summary>
    ///     Gets or sets the <see cref="DbSet{TEntity}" /> of Users.
    /// </summary>
    public DbSet<TUser> Users { get; set; } = null!;

    /// <summary>
    ///     Gets or sets the <see cref="DbSet{TEntity}" /> of User claims.
    /// </summary>
    public DbSet<TUserClaim> UserClaims { get; set; } = null!;

    private StoreOptions? GetStoreOptions()
    {
        return this.GetService<IDbContextOptions>()
            .Extensions.OfType<CoreOptionsExtension>()
            .FirstOrDefault()?.ApplicationServiceProvider
            ?.GetService<IOptions<IdentityOptions>>()
            ?.Value.Stores;
    }

    /// <summary>
    ///     Configures the schema needed for the identity framework.
    /// </summary>
    /// <param name="builder">
    ///     The builder being used to construct the model for this context.
    /// </param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        OnModelCreatingVersion2(builder);
    }

    /// <summary>
    ///     Configures the schema needed for the identity framework for schema version 2.0
    /// </summary>
    /// <param name="builder">
    ///     The builder being used to construct the model for this context.
    /// </param>
    private void OnModelCreatingVersion2(ModelBuilder builder)
    {
        var storeOptions = GetStoreOptions();

        var encryptPersonalData = storeOptions?.ProtectPersonalData ?? false;
        PersonalDataConverter? converter;

        builder.Entity<TUser>(b =>
        {
            b.HasKey(u => u.Id);
            b.HasIndex(u => u.NormalizedUserName).HasDatabaseName("UserNameIndex").IsUnique();
            b.HasIndex(u => u.NormalizedEmail).HasDatabaseName("EmailIndex");
            b.ToTable("AspNetUsers");
            b.Property(u => u.ConcurrencyStamp).IsConcurrencyToken();

            b.Property(u => u.UserName).HasMaxLength(256);
            b.Property(u => u.NormalizedUserName).HasMaxLength(256);
            b.Property(u => u.Email).HasMaxLength(256);
            b.Property(u => u.NormalizedEmail).HasMaxLength(256);

            if (encryptPersonalData)
            {
                converter = new PersonalDataConverter(this.GetService<IPersonalDataProtector>());
                var personalDataProps = typeof(TUser).GetProperties().Where(prop =>
                    Attribute.IsDefined(prop, typeof(ProtectedPersonalDataAttribute)));
                foreach (var p in personalDataProps)
                {
                    if (p.PropertyType != typeof(string))
                    {
                        throw new InvalidOperationException("CanOnlyProtectStrings");
                    }

                    b.Property<string>(p.Name).HasConversion(converter);
                }
            }

            b.HasMany<TUserClaim>().WithOne().HasForeignKey(uc => uc.UserId).IsRequired();
        });

        builder.Entity<TUserClaim>(b =>
        {
            b.HasKey(uc => uc.Id);
            b.ToTable("AspNetUserClaims");
        });
    }

    private sealed class PersonalDataConverter(IPersonalDataProtector protector)
        : ValueConverter<string, string>(s => protector.Protect(s), s => protector.Unprotect(s));
}