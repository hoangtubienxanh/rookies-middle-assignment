// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

using Microsoft.AspNetCore.Identity;

namespace Scribe.AspNetCore.Identity.Extensions.Stores;

/// <summary>
///     Represents a new instance of a persistence store for the specified user type.
/// </summary>
/// <typeparam name="TUser">The type representing a user.</typeparam>
/// <typeparam name="TKey">The type of the primary key for a user.</typeparam>
/// <typeparam name="TUserClaim">The type representing a claim.</typeparam>
[ExcludeFromCodeCoverage(Justification = "derived from Microsoft.AspNetCore.Identity.UserStoreBase")]
public abstract class UserStoreBase<TUser, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TKey,
    TUserClaim> :
    IUserClaimStore<TUser>,
    IUserPasswordStore<TUser>,
    IUserSecurityStampStore<TUser>,
    IUserEmailStore<TUser>,
    IUserLockoutStore<TUser>,
    IQueryableUserStore<TUser>,
    IUserTwoFactorStore<TUser>
    where TUser : User<TKey>
    where TKey : IEquatable<TKey>
    where TUserClaim : UserClaim<TKey>, new()
{
    private const string InternalLoginProvider = "[AspNetUserStore]";
    private const string AuthenticatorKeyTokenName = "AuthenticatorKey";
    private const string RecoveryCodeTokenName = "RecoveryCodes";

    private bool _disposed;

    /// <summary>
    ///     Creates a new instance.
    /// </summary>
    /// <param name="describer">The <see cref="IdentityErrorDescriber" /> used to describe store errors.</param>
    public UserStoreBase(IdentityErrorDescriber describer)
    {
        ArgumentNullException.ThrowIfNull(describer);

        ErrorDescriber = describer;
    }

    /// <summary>
    ///     Gets or sets the <see cref="IdentityErrorDescriber" /> for any error that occurred with the current operation.
    /// </summary>
    public IdentityErrorDescriber ErrorDescriber { get; set; }

    /// <summary>
    ///     A navigation property for the users the store contains.
    /// </summary>
    public abstract IQueryable<TUser> Users
    {
        get;
    }


    /// <summary>
    ///     Gets the user identifier for the specified <paramref name="user" />.
    /// </summary>
    /// <param name="user">The user whose identifier should be retrieved.</param>
    /// <param name="cancellationToken">
    ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
    ///     should be canceled.
    /// </param>
    /// <returns>
    ///     The <see cref="Task" /> that represents the asynchronous operation, containing the identifier for the
    ///     specified <paramref name="user" />.
    /// </returns>
    public virtual Task<string> GetUserIdAsync(TUser user,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(user);
        return Task.FromResult(ConvertIdToString(user.Id)!);
    }

    /// <summary>
    ///     Gets the user name for the specified <paramref name="user" />.
    /// </summary>
    /// <param name="user">The user whose name should be retrieved.</param>
    /// <param name="cancellationToken">
    ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
    ///     should be canceled.
    /// </param>
    /// <returns>
    ///     The <see cref="Task" /> that represents the asynchronous operation, containing the name for the specified
    ///     <paramref name="user" />.
    /// </returns>
    public virtual Task<string?> GetUserNameAsync(TUser user,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(user);
        return Task.FromResult(user.UserName);
    }

    /// <summary>
    ///     Sets the given <paramref name="userName" /> for the specified <paramref name="user" />.
    /// </summary>
    /// <param name="user">The user whose name should be set.</param>
    /// <param name="userName">The user name to set.</param>
    /// <param name="cancellationToken">
    ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
    ///     should be canceled.
    /// </param>
    /// <returns>The <see cref="Task" /> that represents the asynchronous operation.</returns>
    public virtual Task SetUserNameAsync(TUser user, string? userName,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(user);
        user.UserName = userName;
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Gets the normalized user name for the specified <paramref name="user" />.
    /// </summary>
    /// <param name="user">The user whose normalized name should be retrieved.</param>
    /// <param name="cancellationToken">
    ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
    ///     should be canceled.
    /// </param>
    /// <returns>
    ///     The <see cref="Task" /> that represents the asynchronous operation, containing the normalized user name for
    ///     the specified <paramref name="user" />.
    /// </returns>
    public virtual Task<string?> GetNormalizedUserNameAsync(TUser user,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(user);
        return Task.FromResult(user.NormalizedUserName);
    }

    /// <summary>
    ///     Sets the given normalized name for the specified <paramref name="user" />.
    /// </summary>
    /// <param name="user">The user whose name should be set.</param>
    /// <param name="normalizedName">The normalized name to set.</param>
    /// <param name="cancellationToken">
    ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
    ///     should be canceled.
    /// </param>
    /// <returns>The <see cref="Task" /> that represents the asynchronous operation.</returns>
    public virtual Task SetNormalizedUserNameAsync(TUser user, string? normalizedName,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(user);
        user.NormalizedUserName = normalizedName;
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Creates the specified <paramref name="user" /> in the user store.
    /// </summary>
    /// <param name="user">The user to create.</param>
    /// <param name="cancellationToken">
    ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
    ///     should be canceled.
    /// </param>
    /// <returns>
    ///     The <see cref="Task" /> that represents the asynchronous operation, containing the
    ///     <see cref="IdentityResult" /> of the creation operation.
    /// </returns>
    public abstract Task<IdentityResult> CreateAsync(TUser user,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Updates the specified <paramref name="user" /> in the user store.
    /// </summary>
    /// <param name="user">The user to update.</param>
    /// <param name="cancellationToken">
    ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
    ///     should be canceled.
    /// </param>
    /// <returns>
    ///     The <see cref="Task" /> that represents the asynchronous operation, containing the
    ///     <see cref="IdentityResult" /> of the update operation.
    /// </returns>
    public abstract Task<IdentityResult> UpdateAsync(TUser user,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes the specified <paramref name="user" /> from the user store.
    /// </summary>
    /// <param name="user">The user to delete.</param>
    /// <param name="cancellationToken">
    ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
    ///     should be canceled.
    /// </param>
    /// <returns>
    ///     The <see cref="Task" /> that represents the asynchronous operation, containing the
    ///     <see cref="IdentityResult" /> of the update operation.
    /// </returns>
    public abstract Task<IdentityResult> DeleteAsync(TUser user,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Finds and returns a user, if any, who has the specified <paramref name="userId" />.
    /// </summary>
    /// <param name="userId">The user ID to search for.</param>
    /// <param name="cancellationToken">
    ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
    ///     should be canceled.
    /// </param>
    /// <returns>
    ///     The <see cref="Task" /> that represents the asynchronous operation, containing the user matching the specified
    ///     <paramref name="userId" /> if it exists.
    /// </returns>
    public abstract Task<TUser?> FindByIdAsync(string userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Finds and returns a user, if any, who has the specified normalized user name.
    /// </summary>
    /// <param name="normalizedUserName">The normalized user name to search for.</param>
    /// <param name="cancellationToken">
    ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
    ///     should be canceled.
    /// </param>
    /// <returns>
    ///     The <see cref="Task" /> that represents the asynchronous operation, containing the user matching the specified
    ///     <paramref name="normalizedUserName" /> if it exists.
    /// </returns>
    public abstract Task<TUser?> FindByNameAsync(string normalizedUserName,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Dispose the store
    /// </summary>
    public void Dispose()
    {
        _disposed = true;
    }

    /// <summary>
    ///     Get the claims associated with the specified <paramref name="user" /> as an asynchronous operation.
    /// </summary>
    /// <param name="user">The user whose claims should be retrieved.</param>
    /// <param name="cancellationToken">
    ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
    ///     should be canceled.
    /// </param>
    /// <returns>A <see cref="Task{TResult}" /> that contains the claims granted to a user.</returns>
    public abstract Task<IList<Claim>> GetClaimsAsync(TUser user,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Adds the <paramref name="claims" /> given to the specified <paramref name="user" />.
    /// </summary>
    /// <param name="user">The user to add the claim to.</param>
    /// <param name="claims">The claim to add to the user.</param>
    /// <param name="cancellationToken">
    ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
    ///     should be canceled.
    /// </param>
    /// <returns>The <see cref="Task" /> that represents the asynchronous operation.</returns>
    public abstract Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Replaces the <paramref name="claim" /> on the specified <paramref name="user" />, with the
    ///     <paramref name="newClaim" />.
    /// </summary>
    /// <param name="user">The user to replace the claim on.</param>
    /// <param name="claim">The claim replace.</param>
    /// <param name="newClaim">The new claim replacing the <paramref name="claim" />.</param>
    /// <param name="cancellationToken">
    ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
    ///     should be canceled.
    /// </param>
    /// <returns>The <see cref="Task" /> that represents the asynchronous operation.</returns>
    public abstract Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Removes the <paramref name="claims" /> given from the specified <paramref name="user" />.
    /// </summary>
    /// <param name="user">The user to remove the claims from.</param>
    /// <param name="claims">The claim to remove.</param>
    /// <param name="cancellationToken">
    ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
    ///     should be canceled.
    /// </param>
    /// <returns>The <see cref="Task" /> that represents the asynchronous operation.</returns>
    public abstract Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves all users with the specified claim.
    /// </summary>
    /// <param name="claim">The claim whose users should be retrieved.</param>
    /// <param name="cancellationToken">
    ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
    ///     should be canceled.
    /// </param>
    /// <returns>
    ///     The <see cref="Task" /> contains a list of users, if any, that contain the specified claim.
    /// </returns>
    public abstract Task<IList<TUser>> GetUsersForClaimAsync(Claim claim,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets a flag indicating whether the email address for the specified <paramref name="user" /> has been verified, true
    ///     if the email address is verified otherwise
    ///     false.
    /// </summary>
    /// <param name="user">The user whose email confirmation status should be returned.</param>
    /// <param name="cancellationToken">
    ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
    ///     should be canceled.
    /// </param>
    /// <returns>
    ///     The task object containing the results of the asynchronous operation, a flag indicating whether the email address
    ///     for the specified <paramref name="user" />
    ///     has been confirmed or not.
    /// </returns>
    public virtual Task<bool> GetEmailConfirmedAsync(TUser user,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(user);
        return Task.FromResult(user.EmailConfirmed);
    }

    /// <summary>
    ///     Sets the flag indicating whether the specified <paramref name="user" />'s email address has been confirmed or not.
    /// </summary>
    /// <param name="user">The user whose email confirmation status should be set.</param>
    /// <param name="confirmed">
    ///     A flag indicating if the email address has been confirmed, true if the address is confirmed
    ///     otherwise false.
    /// </param>
    /// <param name="cancellationToken">
    ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
    ///     should be canceled.
    /// </param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    public virtual Task SetEmailConfirmedAsync(TUser user, bool confirmed,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(user);
        user.EmailConfirmed = confirmed;
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Sets the <paramref name="email" /> address for a <paramref name="user" />.
    /// </summary>
    /// <param name="user">The user whose email should be set.</param>
    /// <param name="email">The email to set.</param>
    /// <param name="cancellationToken">
    ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
    ///     should be canceled.
    /// </param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    public virtual Task SetEmailAsync(TUser user, string? email,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(user);
        user.Email = email;
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Gets the email address for the specified <paramref name="user" />.
    /// </summary>
    /// <param name="user">The user whose email should be returned.</param>
    /// <param name="cancellationToken">
    ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
    ///     should be canceled.
    /// </param>
    /// <returns>
    ///     The task object containing the results of the asynchronous operation, the email address for the specified
    ///     <paramref name="user" />.
    /// </returns>
    public virtual Task<string?> GetEmailAsync(TUser user,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(user);
        return Task.FromResult(user.Email);
    }

    /// <summary>
    ///     Returns the normalized email for the specified <paramref name="user" />.
    /// </summary>
    /// <param name="user">The user whose email address to retrieve.</param>
    /// <param name="cancellationToken">
    ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
    ///     should be canceled.
    /// </param>
    /// <returns>
    ///     The task object containing the results of the asynchronous lookup operation, the normalized email address if any
    ///     associated with the specified user.
    /// </returns>
    public virtual Task<string?> GetNormalizedEmailAsync(TUser user,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(user);
        return Task.FromResult(user.NormalizedEmail);
    }

    /// <summary>
    ///     Sets the normalized email for the specified <paramref name="user" />.
    /// </summary>
    /// <param name="user">The user whose email address to set.</param>
    /// <param name="normalizedEmail">The normalized email to set for the specified <paramref name="user" />.</param>
    /// <param name="cancellationToken">
    ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
    ///     should be canceled.
    /// </param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    public virtual Task SetNormalizedEmailAsync(TUser user, string? normalizedEmail,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(user);
        user.NormalizedEmail = normalizedEmail;
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Gets the user, if any, associated with the specified, normalized email address.
    /// </summary>
    /// <param name="normalizedEmail">The normalized email address to return the user for.</param>
    /// <param name="cancellationToken">
    ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
    ///     should be canceled.
    /// </param>
    /// <returns>
    ///     The task object containing the results of the asynchronous lookup operation, the user if any associated with the
    ///     specified normalized email address.
    /// </returns>
    public abstract Task<TUser?> FindByEmailAsync(string normalizedEmail,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets the last <see cref="DateTimeOffset" /> a user's last lockout expired, if any.
    ///     Any time in the past should be indicates a user is not locked out.
    /// </summary>
    /// <param name="user">The user whose lockout date should be retrieved.</param>
    /// <param name="cancellationToken">
    ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
    ///     should be canceled.
    /// </param>
    /// <returns>
    ///     A <see cref="Task{TResult}" /> that represents the result of the asynchronous query, a
    ///     <see cref="DateTimeOffset" /> containing the last time
    ///     a user's lockout expired, if any.
    /// </returns>
    public virtual Task<DateTimeOffset?> GetLockoutEndDateAsync(TUser user,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(user);
        return Task.FromResult(user.LockoutEnd);
    }

    /// <summary>
    ///     Locks out a user until the specified end date has passed. Setting a end date in the past immediately unlocks a
    ///     user.
    /// </summary>
    /// <param name="user">The user whose lockout date should be set.</param>
    /// <param name="lockoutEnd">
    ///     The <see cref="DateTimeOffset" /> after which the <paramref name="user" />'s lockout should
    ///     end.
    /// </param>
    /// <param name="cancellationToken">
    ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
    ///     should be canceled.
    /// </param>
    /// <returns>The <see cref="Task" /> that represents the asynchronous operation.</returns>
    public virtual Task SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(user);
        user.LockoutEnd = lockoutEnd;
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Records that a failed access has occurred, incrementing the failed access count.
    /// </summary>
    /// <param name="user">The user whose cancellation count should be incremented.</param>
    /// <param name="cancellationToken">
    ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
    ///     should be canceled.
    /// </param>
    /// <returns>
    ///     The <see cref="Task" /> that represents the asynchronous operation, containing the incremented failed access
    ///     count.
    /// </returns>
    public virtual Task<int> IncrementAccessFailedCountAsync(TUser user,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(user);
        user.AccessFailedCount++;
        return Task.FromResult(user.AccessFailedCount);
    }

    /// <summary>
    ///     Resets a user's failed access count.
    /// </summary>
    /// <param name="user">The user whose failed access count should be reset.</param>
    /// <param name="cancellationToken">
    ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
    ///     should be canceled.
    /// </param>
    /// <returns>The <see cref="Task" /> that represents the asynchronous operation.</returns>
    /// <remarks>This is typically called after the account is successfully accessed.</remarks>
    public virtual Task ResetAccessFailedCountAsync(TUser user,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(user);
        user.AccessFailedCount = 0;
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Retrieves the current failed access count for the specified <paramref name="user" />..
    /// </summary>
    /// <param name="user">The user whose failed access count should be retrieved.</param>
    /// <param name="cancellationToken">
    ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
    ///     should be canceled.
    /// </param>
    /// <returns>The <see cref="Task" /> that represents the asynchronous operation, containing the failed access count.</returns>
    public virtual Task<int> GetAccessFailedCountAsync(TUser user,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(user);
        return Task.FromResult(user.AccessFailedCount);
    }

    /// <summary>
    ///     Retrieves a flag indicating whether user lockout can enabled for the specified user.
    /// </summary>
    /// <param name="user">The user whose ability to be locked out should be returned.</param>
    /// <param name="cancellationToken">
    ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
    ///     should be canceled.
    /// </param>
    /// <returns>
    ///     The <see cref="Task" /> that represents the asynchronous operation, true if a user can be locked out, otherwise
    ///     false.
    /// </returns>
    public virtual Task<bool> GetLockoutEnabledAsync(TUser user,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(user);
        return Task.FromResult(user.LockoutEnabled);
    }

    /// <summary>
    ///     Set the flag indicating if the specified <paramref name="user" /> can be locked out..
    /// </summary>
    /// <param name="user">The user whose ability to be locked out should be set.</param>
    /// <param name="enabled">A flag indicating if lock out can be enabled for the specified <paramref name="user" />.</param>
    /// <param name="cancellationToken">
    ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
    ///     should be canceled.
    /// </param>
    /// <returns>The <see cref="Task" /> that represents the asynchronous operation.</returns>
    public virtual Task SetLockoutEnabledAsync(TUser user, bool enabled,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(user);
        user.LockoutEnabled = enabled;
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Sets the password hash for a user.
    /// </summary>
    /// <param name="user">The user to set the password hash for.</param>
    /// <param name="passwordHash">The password hash to set.</param>
    /// <param name="cancellationToken">
    ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
    ///     should be canceled.
    /// </param>
    /// <returns>The <see cref="Task" /> that represents the asynchronous operation.</returns>
    public virtual Task SetPasswordHashAsync(TUser user, string? passwordHash,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(user);
        user.PasswordHash = passwordHash;
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Gets the password hash for a user.
    /// </summary>
    /// <param name="user">The user to retrieve the password hash for.</param>
    /// <param name="cancellationToken">
    ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
    ///     should be canceled.
    /// </param>
    /// <returns>A <see cref="Task{TResult}" /> that contains the password hash for the user.</returns>
    public virtual Task<string?> GetPasswordHashAsync(TUser user,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(user);
        return Task.FromResult(user.PasswordHash);
    }

    /// <summary>
    ///     Returns a flag indicating if the specified user has a password.
    /// </summary>
    /// <param name="user">The user to retrieve the password hash for.</param>
    /// <param name="cancellationToken">
    ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
    ///     should be canceled.
    /// </param>
    /// <returns>
    ///     A <see cref="Task{TResult}" /> containing a flag indicating if the specified user has a password. If the
    ///     user has a password the returned value with be true, otherwise it will be false.
    /// </returns>
    public virtual Task<bool> HasPasswordAsync(TUser user,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(user.PasswordHash != null);
    }


    /// <summary>
    ///     Sets the provided security <paramref name="stamp" /> for the specified <paramref name="user" />.
    /// </summary>
    /// <param name="user">The user whose security stamp should be set.</param>
    /// <param name="stamp">The security stamp to set.</param>
    /// <param name="cancellationToken">
    ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
    ///     should be canceled.
    /// </param>
    /// <returns>The <see cref="Task" /> that represents the asynchronous operation.</returns>
    public virtual Task SetSecurityStampAsync(TUser user, string stamp,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(stamp);
        user.SecurityStamp = stamp;
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Get the security stamp for the specified <paramref name="user" />.
    /// </summary>
    /// <param name="user">The user whose security stamp should be set.</param>
    /// <param name="cancellationToken">
    ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
    ///     should be canceled.
    /// </param>
    /// <returns>
    ///     The <see cref="Task" /> that represents the asynchronous operation, containing the security stamp for the
    ///     specified <paramref name="user" />.
    /// </returns>
    public virtual Task<string?> GetSecurityStampAsync(TUser user,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(user);
        return Task.FromResult(user.SecurityStamp);
    }

    /// <summary>
    ///     Sets a flag indicating whether the specified <paramref name="user" /> has two factor authentication enabled or not,
    ///     as an asynchronous operation.
    /// </summary>
    /// <param name="user">The user whose two factor authentication enabled status should be set.</param>
    /// <param name="enabled">
    ///     A flag indicating whether the specified <paramref name="user" /> has two factor authentication
    ///     enabled.
    /// </param>
    /// <param name="cancellationToken">
    ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
    ///     should be canceled.
    /// </param>
    /// <returns>The <see cref="Task" /> that represents the asynchronous operation.</returns>
    public virtual Task SetTwoFactorEnabledAsync(TUser user, bool enabled,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(user);
        user.TwoFactorEnabled = enabled;
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Returns a flag indicating whether the specified <paramref name="user" /> has two factor authentication enabled or
    ///     not,
    ///     as an asynchronous operation.
    /// </summary>
    /// <param name="user">The user whose two factor authentication enabled status should be set.</param>
    /// <param name="cancellationToken">
    ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
    ///     should be canceled.
    /// </param>
    /// <returns>
    ///     The <see cref="Task" /> that represents the asynchronous operation, containing a flag indicating whether the
    ///     specified
    ///     <paramref name="user" /> has two factor authentication enabled or not.
    /// </returns>
    public virtual Task<bool> GetTwoFactorEnabledAsync(TUser user,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(user);
        return Task.FromResult(user.TwoFactorEnabled);
    }

    /// <summary>
    ///     Called to create a new instance of a <see cref="IdentityUserClaim{TKey}" />.
    /// </summary>
    /// <param name="user">The associated user.</param>
    /// <param name="claim">The associated claim.</param>
    /// <returns></returns>
    protected virtual TUserClaim CreateUserClaim(TUser user, Claim claim)
    {
        var userClaim = new TUserClaim { UserId = user.Id };
        userClaim.InitializeFromClaim(claim);
        return userClaim;
    }

    /// <summary>
    ///     Converts the provided <paramref name="id" /> to a strongly typed key object.
    /// </summary>
    /// <param name="id">The id to convert.</param>
    /// <returns>An instance of <typeparamref name="TKey" /> representing the provided <paramref name="id" />.</returns>
    [UnconditionalSuppressMessage("Trimming",
        "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
        Justification = "TKey is annoated with RequiresUnreferencedCodeAttribute.All.")]
    public virtual TKey? ConvertIdFromString(string? id)
    {
        if (id == null)
        {
            return default;
        }

        return (TKey?)TypeDescriptor.GetConverter(typeof(TKey)).ConvertFromInvariantString(id);
    }

    /// <summary>
    ///     Converts the provided <paramref name="id" /> to its string representation.
    /// </summary>
    /// <param name="id">The id to convert.</param>
    /// <returns>An <see cref="string" /> representation of the provided <paramref name="id" />.</returns>
    public virtual string? ConvertIdToString(TKey id)
    {
        if (Equals(id, default(TKey)))
        {
            return null;
        }

        return id.ToString();
    }

    /// <summary>
    ///     Return a user with the matching userId if it exists.
    /// </summary>
    /// <param name="userId">The user's id.</param>
    /// <param name="cancellationToken">
    ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
    ///     should be canceled.
    /// </param>
    /// <returns>The user if it exists.</returns>
    protected abstract Task<TUser?> FindUserAsync(TKey userId, CancellationToken cancellationToken);
}