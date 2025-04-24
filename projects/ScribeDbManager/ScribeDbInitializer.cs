using System.Diagnostics;

using Microsoft.EntityFrameworkCore;

using Scribe;

namespace ScribeDbManager;

internal class ScribeDbInitializer(IServiceProvider serviceProvider, ILogger<ScribeDbInitializer> logger)
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
    }
}