// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Scribe.AspNetCore.Identity.Extensions.Stores;

namespace Scribe.AspNetCore.Identity.EntityFrameworkCore;

/// <summary>
///     Contains extension methods to <see cref="IdentityBuilder" /> for adding entity framework stores.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "derived from IdentityEntityFrameworkBuilderExtensions")]
public static class IdentityEntityFrameworkBuilderExtensions
{
    /// <summary>
    ///     Adds an Entity Framework implementation of identity information stores.
    /// </summary>
    /// <typeparam name="TContext">The Entity Framework database context to use.</typeparam>
    /// <param name="builder">The <see cref="IdentityBuilder" /> instance this method extends.</param>
    /// <returns>The <see cref="IdentityBuilder" /> instance this method extends.</returns>
    public static IdentityBuilder AddEntityFrameworkStores<TContext>(this IdentityBuilder builder)
        where TContext : DbContext
    {
        AddStores(builder.Services, builder.UserType, typeof(TContext));
        return builder;
    }

    private static void AddStores(IServiceCollection services, Type userType, Type contextType)
    {
        var identityUserType =
            FindGenericBaseType(userType, typeof(User<>));
        if (identityUserType == null)
        {
            throw new InvalidOperationException("Resources.NotIdentityUser");
        }

        var keyType = identityUserType.GenericTypeArguments[0];

        // No roles
        // If it's a custom DbContext, we can only add the default POCOs
        var userStoreType = typeof(UserOnlyStore<,,>).MakeGenericType(userType, contextType, keyType);
        services.TryAddScoped(typeof(IUserStore<>).MakeGenericType(userType), userStoreType);
    }

    private static Type? FindGenericBaseType(Type currentType, Type genericBaseType)
    {
        var type = currentType;
        while (type != null)
        {
            var genericType = type.IsGenericType ? type.GetGenericTypeDefinition() : null;
            if (genericType != null && genericType == genericBaseType)
            {
                return type;
            }

            type = type.BaseType;
        }

        return null;
    }
}