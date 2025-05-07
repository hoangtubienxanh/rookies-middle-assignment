using System.Diagnostics;
using System.Security.Claims;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using Scribe.EntityFrameworkCore;
using Scribe.EntityFrameworkCore.Stores;

namespace ScribeDbManager;

internal class DbInitializer(IServiceProvider serviceProvider, ILogger<DbInitializer> logger)
    : BackgroundService
{
    public const string ActivitySourceName = "Migrations";

    private readonly ActivitySource _activitySource = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ScribeContext>();

        using var activity = _activitySource.StartActivity(ActivityKind.Client);
        if (activity != null)
        {
            activity.DisplayName = "Initializing scribe database";
        }

        await InitializeDatabaseAsync(dbContext, cancellationToken);
    }

    public async Task InitializeDatabaseAsync(ScribeContext dbContext,
        CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();

        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(dbContext.Database.MigrateAsync, cancellationToken);

        await SeedAsync(dbContext, cancellationToken);

        logger.LogInformation("Database initialization completed after {ElapsedMilliseconds}ms",
            sw.ElapsedMilliseconds);
    }

    private async Task SeedAsync(ScribeContext dbContext, CancellationToken cancellationToken)
    {
        logger.LogInformation("Seeding database");

        if (!dbContext.Categories.Any())
        {
            dbContext.Categories.AddRange(
                new Category { Name = "Fiction", Slug = "fiction" },
                new Category { Name = "Non-Fiction", Slug = "non-fiction" });
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        if (!dbContext.Books.Any())
        {
            dbContext.Books.AddRange(
                new Book
                {
                    Title = "Book 1",
                    Author = "Author 1",
                    Quantity = 10,
                    CategoryId = dbContext.Categories.First().CategoryId
                },
                new Book
                {
                    Title = "Book 2",
                    Author = "Author 2",
                    Quantity = 5,
                    CategoryId = dbContext.Categories.First().CategoryId
                },
                new Book
                {
                    Title = "Book 3",
                    Author = "Author 3",
                    Quantity = 1,
                    CategoryId = dbContext.Categories.First().CategoryId
                });
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        var userOnlyStore = new UserOnlyStore<ScribeUser, ScribeContext, Guid>(dbContext);
        var passwordHasher = new PasswordHasher<ScribeUser>();
        var userManager =
            new UserManager<ScribeUser>(userOnlyStore, null, passwordHasher, null, null, null, null, null, null);

        if (!dbContext.Users.Any(u => u.UserName == "admin@scribe.local"))
        {
            var adminUser = new ScribeUser
            {
                UserName = "admin@scribe.local", Email = "admin@scribe.local", EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, "weakPassword");
            if (result.Succeeded)
            {
                await userManager.AddClaimAsync(adminUser, new Claim(ClaimTypes.Role, "administrator"));
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Database seeding completed");
    }
}