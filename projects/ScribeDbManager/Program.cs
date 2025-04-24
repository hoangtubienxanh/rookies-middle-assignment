using Microsoft.EntityFrameworkCore;

using Scribe;

using ScribeDbManager;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddSqliteDbContext<ScribeContext>("scribe", configureDbContextOptions: dbContextOptionsBuilder =>
{
    var connectionString = builder.Configuration.GetConnectionString("scribe");
    if (string.IsNullOrEmpty(connectionString) && !EF.IsDesignTime)
    {
        // allows empty connection string on ef migrations
        throw new InvalidOperationException("Connection string 'scribe' not found.");
    }

    dbContextOptionsBuilder.UseSqlite(sqliteBuilder =>
        sqliteBuilder.MigrationsAssembly(typeof(Program).Assembly.GetName().Name));
});

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(ScribeDbInitializer.ActivitySourceName));

builder.Services.AddSingleton<ScribeDbInitializer>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<ScribeDbInitializer>());
builder.Services.AddHealthChecks().AddCheck<ScribeDbInitializerHealthCheck>("DbInitializer");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapPost("/reset-db",
        async (ScribeContext dbContext, ScribeDbInitializer dbInitializer,
            CancellationToken cancellationToken) =>
        {
            // Delete and recreate the database. This is useful for development scenarios to reset the database to its initial state.
            await dbContext.Database.EnsureDeletedAsync(cancellationToken);
            await dbInitializer.InitializeDatabaseAsync(dbContext, cancellationToken);
        });
}

app.MapDefaultEndpoints();

await app.RunAsync();