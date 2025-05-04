using System.Diagnostics;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Options;

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
        await BootstrapAdminIdentity(dbContext, cancellationToken);

        logger.LogInformation("Database initialization completed after {ElapsedMilliseconds}ms",
            sw.ElapsedMilliseconds);
    }

    private async Task SeedAsync(ScribeContext dbContext, CancellationToken cancellationToken)
    {
        logger.LogInformation("Seeding database");
    }

    public async Task BootstrapAdminIdentity(ScribeContext dbContext, CancellationToken cancellationToken)
    {
        logger.LogInformation("Seeding admin user");

        var sp = dbContext.GetInfrastructure().GetRequiredService<IServiceProvider>();
        var userManager = sp.GetRequiredService<UserManager<ScribeUser>>();
        var lockoutManager = sp.GetRequiredService<IUserLockoutStore<ScribeUser>>();
        var timeBeforeLockout =
            sp.GetRequiredService<IOptions<SecurityStampValidatorOptions>>().Value.ValidationInterval;

        var user = new ScribeUser();
        await userManager.CreateAsync(user, "$$$myVery0wnPassword");
        await userManager.AddToRoleAsync(user, "SUPER");

        await lockoutManager.SetLockoutEnabledAsync(user, true, cancellationToken);
        await lockoutManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue, cancellationToken);
    }
}