// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;

using Microsoft.AspNetCore.Identity;

namespace Scribe.AspNetCore.Identity.Extensions.Stores;

/// <summary>
///     The default implementation of <see cref="User{TKey}" /> which uses a string as a primary key.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "derived from Microsoft.AspNetCore.Identity.IdentityUser")]
public class User : User<string>
{
    /// <summary>
    ///     Initializes a new instance of <see cref="User" />.
    /// </summary>
    /// <remarks>
    ///     The Id property is initialized to form a new GUID string value.
    /// </remarks>
    public User()
    {
        Id = Guid.CreateVersion7().ToString();
        SecurityStamp = Guid.NewGuid().ToString();
    }

    /// <summary>
    ///     Initializes a new instance of <see cref="User" />.
    /// </summary>
    /// <param name="userName">The user name.</param>
    /// <remarks>
    ///     The Id property is initialized to form a new GUID string value.
    /// </remarks>
    public User(string userName) : this()
    {
        UserName = userName;
    }
}

/// <summary>
///     Represents a user in the identity system
/// </summary>
/// <typeparam name="TKey">The type used for the primary key for the user.</typeparam>
public class User<TKey> where TKey : IEquatable<TKey>
{
    /// <summary>
    ///     Initializes a new instance of <see cref="User{TKey}" />.
    /// </summary>
    public User() { }

    /// <summary>
    ///     Initializes a new instance of <see cref="User{TKey}" />.
    /// </summary>
    /// <param name="userName">The user name.</param>
    public User(string userName) : this()
    {
        UserName = userName;
    }

    /// <summary>
    ///     Gets or sets the primary key for this user.
    /// </summary>
    [PersonalData]
    public virtual TKey Id { get; set; } = default!;

    /// <summary>
    ///     Gets or sets the user name for this user.
    /// </summary>
    [ProtectedPersonalData]
    public virtual string? UserName { get; set; }

    /// <summary>
    ///     Gets or sets the normalized user name for this user.
    /// </summary>
    public virtual string? NormalizedUserName { get; set; }

    /// <summary>
    ///     Gets or sets the email address for this user.
    /// </summary>
    [ProtectedPersonalData]
    public virtual string? Email { get; set; }

    /// <summary>
    ///     Gets or sets the normalized email address for this user.
    /// </summary>
    public virtual string? NormalizedEmail { get; set; }

    /// <summary>
    ///     Gets or sets a flag indicating if a user has confirmed their email address.
    /// </summary>
    /// <value>True if the email address has been confirmed, otherwise false.</value>
    [PersonalData]
    public virtual bool EmailConfirmed { get; set; }

    /// <summary>
    ///     Gets or sets a salted and hashed representation of the password for this user.
    /// </summary>
    public virtual string? PasswordHash { get; set; }

    /// <summary>
    ///     A random value that must change whenever a users credentials change (password changed, login removed)
    /// </summary>
    public virtual string? SecurityStamp { get; set; }

    /// <summary>
    ///     A random value that must change whenever a user is persisted to the store
    /// </summary>
    public virtual string? ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    ///     Gets or sets a flag indicating if two factor authentication is enabled for this user.
    /// </summary>
    /// <value>True if 2fa is enabled, otherwise false.</value>
    [PersonalData]
    public virtual bool TwoFactorEnabled { get; set; }

    /// <summary>
    ///     Gets or sets the date and time, in UTC, when any user lockout ends.
    /// </summary>
    /// <remarks>
    ///     A value in the past means the user is not locked out.
    /// </remarks>
    public virtual DateTimeOffset? LockoutEnd { get; set; }

    /// <summary>
    ///     Gets or sets a flag indicating if the user could be locked out.
    /// </summary>
    /// <value>True if the user could be locked out, otherwise false.</value>
    public virtual bool LockoutEnabled { get; set; }

    /// <summary>
    ///     Gets or sets the number of failed login attempts for the current user.
    /// </summary>
    public virtual int AccessFailedCount { get; set; }

    /// <summary>
    ///     Returns the username for this user.
    /// </summary>
    public override string ToString()
    {
        return UserName ?? string.Empty;
    }
}