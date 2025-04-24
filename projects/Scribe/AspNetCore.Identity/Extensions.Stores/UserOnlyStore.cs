// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Scribe.AspNetCore.Identity.Extensions.Stores;

/// <summary>
///     Represents a new instance of a persistence store for the specified user and role types.
/// </summary>
/// <typeparam name="TUser">The type representing a user.</typeparam>
/// <typeparam name="TContext">The type of the data context class used to access the store.</typeparam>
[ExcludeFromCodeCoverage(Justification = "derived from Microsoft.AspNetCore.Identity.EntityFrameworkCore.UserOnlyStore")]
public class UserOnlyStore<TUser, TContext> : UserOnlyStore<TUser, TContext, string>
    where TUser : User<string>
    where TContext : DbContext
{
    /// <summary>
    ///     Constructs a new instance of <see cref="UserOnlyStore{TUser, TRole, TContext}" />.
    /// </summary>
    /// <param name="context">The <see cref="DbContext" />.</param>
    /// <param name="describer">The <see cref="IdentityErrorDescriber" />.</param>
    public UserOnlyStore(TContext context, IdentityErrorDescriber? describer = null) : base(context, describer) { }
}

/// <summary>
///     Represents a new instance of a persistence store for the specified user and role types.
/// </summary>
/// <typeparam name="TUser">The type representing a user.</typeparam>
/// <typeparam name="TContext">The type of the data context class used to access the store.</typeparam>
/// <typeparam name="TKey">The type of the primary key for a role.</typeparam>
[ExcludeFromCodeCoverage(Justification = "derived from Microsoft.AspNetCore.Identity.EntityFrameworkCore.UserOnlyStore")]
public class UserOnlyStore<TUser, TContext, TKey> : UserOnlyStore<TUser, TContext, TKey, UserClaim<TKey>>
    where TUser : User<TKey>
    where TContext : DbContext
    where TKey : IEquatable<TKey>
{
    /// <summary>
    ///     Constructs a new instance of <see cref="UserOnlyStore{TUser, TContext, TKey, TUserClaim}" />.
    /// </summary>
    /// <param name="context">The <see cref="DbContext" />.</param>
    /// <param name="describer">The <see cref="IdentityErrorDescriber" />.</param>
    public UserOnlyStore(TContext context, IdentityErrorDescriber? describer = null) : base(context, describer) { }
}

/// <summary>
///     Represents a new instance of a persistence store for the specified user types.
/// </summary>
/// <typeparam name="TUser">The type representing a user.</typeparam>
/// <typeparam name="TContext">The type of the data context class used to access the store.</typeparam>
/// <typeparam name="TKey">The type of the primary key for a role.</typeparam>
/// <typeparam name="TUserClaim">The type representing a claim.</typeparam>
/// <typeparam name="TUserLogin">The type representing a user external login.</typeparam>
/// <typeparam name="TUserToken">The type representing a user token.</typeparam>
[ExcludeFromCodeCoverage(Justification = "derived from Microsoft.AspNetCore.Identity.EntityFrameworkCore.UserOnlyStore")]
public class UserOnlyStore<TUser, TContext, TKey, TUserClaim> :
    UserStoreBase<TUser, TKey, TUserClaim>
    where TUser : User<TKey>
    where TContext : DbContext
    where TKey : IEquatable<TKey>
    where TUserClaim : UserClaim<TKey>, new()
{
    /// <summary>
    ///     Creates a new instance of the store.
    /// </summary>
    /// <param name="context">The context used to access the store.</param>
    /// <param name="describer">The <see cref="IdentityErrorDescriber" /> used to describe store errors.</param>
    public UserOnlyStore(TContext context, IdentityErrorDescriber? describer = null) : base(describer ??
        new IdentityErrorDescriber())
    {
        ArgumentNullException.ThrowIfNull(context);
        Context = context;
    }

    /// <summary>
    ///     Gets the database context for this store.
    /// </summary>
    public virtual TContext Context { get; }

    /// <summary>
    ///     DbSet of users.
    /// </summary>
    protected DbSet<TUser> UsersSet => Context.Set<TUser>();

    /// <summary>
    ///     DbSet of user claims.
    /// </summary>
    protected DbSet<TUserClaim> UserClaims => Context.Set<TUserClaim>();

    /// <summary>
    ///     Gets or sets a flag indicating if changes should be persisted after CreateAsync, UpdateAsync and DeleteAsync are
    ///     called.
    /// </summary>
    /// <value>
    ///     True if changes should be automatically persisted, otherwise false.
    /// </value>
    public bool AutoSaveChanges { get; set; } = true;

    /// <summary>
    ///     A navigation property for the users the store contains.
    /// </summary>
    public override IQueryable<TUser> Users => UsersSet;


    /// <summary>
    ///     Get the claims associated with the specified <paramref name="user" /> as an asynchronous operation.
    /// </summary>
    /// <param name="user">The user whose claims should be retrieved.</param>
    /// <param name="cancellationToken">
    ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
    ///     should be canceled.
    /// </param>
    /// <returns>A <see cref="Task{TResult}" /> that contains the claims granted to a user.</returns>
    public override async Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);

        return await UserClaims.Where(uc => uc.UserId.Equals(user.Id)).Select(c => c.ToClaim())
            .ToListAsync(cancellationToken);
    }

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
    public override Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(claims);
        foreach (var claim in claims)
        {
            UserClaims.Add(CreateUserClaim(user, claim));
        }

        return Task.FromResult(false);
    }


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
    public override async Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(claim);
        ArgumentNullException.ThrowIfNull(newClaim);

        var matchedClaims = await UserClaims
            .Where(uc => uc.UserId.Equals(user.Id) && uc.ClaimValue == claim.Value && uc.ClaimType == claim.Type)
            .ToListAsync(cancellationToken);
        foreach (var matchedClaim in matchedClaims)
        {
            matchedClaim.ClaimValue = newClaim.Value;
            matchedClaim.ClaimType = newClaim.Type;
        }
    }

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
    public override async Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(claims);
        foreach (var claim in claims)
        {
            var matchedClaims = await UserClaims
                .Where(uc => uc.UserId.Equals(user.Id) && uc.ClaimValue == claim.Value && uc.ClaimType == claim.Type)
                .ToListAsync(cancellationToken);
            foreach (var c in matchedClaims)
            {
                UserClaims.Remove(c);
            }
        }
    }

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
    public override async Task<IList<TUser>> GetUsersForClaimAsync(Claim claim,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(claim);

        var query = from userclaims in UserClaims
            join user in Users on userclaims.UserId equals user.Id
            where userclaims.ClaimValue == claim.Value
                  && userclaims.ClaimType == claim.Type
            select user;

        return await query.ToListAsync(cancellationToken);
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
    public override Task<TUser?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();


        return Users.SingleOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);
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
    public override async Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(user);
        Context.Add(user);
        await SaveChanges(cancellationToken);
        return IdentityResult.Success;
    }

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
    public override async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(user);

        Context.Attach(user);
        user.ConcurrencyStamp = Guid.NewGuid().ToString();
        Context.Update(user);
        try
        {
            await SaveChanges(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
        }

        return IdentityResult.Success;
    }

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
    public override async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ArgumentNullException.ThrowIfNull(user);

        Context.Remove(user);
        try
        {
            await SaveChanges(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
        }

        return IdentityResult.Success;
    }

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
    public override Task<TUser?> FindByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var id = ConvertIdFromString(userId);
        return UsersSet.FindAsync(new object?[] { id }, cancellationToken).AsTask();
    }

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
    public override Task<TUser?> FindByNameAsync(string normalizedUserName,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();


        return Users.FirstOrDefaultAsync(u => u.NormalizedUserName == normalizedUserName, cancellationToken);
    }

    /// <summary>Saves the current store.</summary>
    /// <param name="cancellationToken">
    ///     The <see cref="CancellationToken" /> used to propagate notifications that the operation
    ///     should be canceled.
    /// </param>
    /// <returns>The <see cref="Task" /> that represents the asynchronous operation.</returns>
    protected Task SaveChanges(CancellationToken cancellationToken)
    {
        return AutoSaveChanges ? Context.SaveChangesAsync(cancellationToken) : Task.CompletedTask;
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
    protected override Task<TUser?> FindUserAsync(TKey userId, CancellationToken cancellationToken)
    {
        return Users.SingleOrDefaultAsync(u => u.Id.Equals(userId), cancellationToken);
    }
}