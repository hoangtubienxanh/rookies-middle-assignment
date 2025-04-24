using System.Reflection;

using Api.Endpoints;

using Microsoft.AspNetCore.Identity;

using Scribe;
using Scribe.AspNetCore.Identity.EntityFrameworkCore;
using Scribe.AspNetCore.Identity.Extensions.Stores;

namespace Api;

public static class Extensions
{
    public static TBuilder AddApplicationServices<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        builder.AddEndpointServices();

        // Customizing run-time behavior during build-time document generation
        // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/aspnetcore-openapi#customizing-run-time-behavior-during-build-time-document-generation
        if (Assembly.GetEntryAssembly()?.GetName().Name == "GetDocument.Insider")
        {
            return builder;
        }

        builder.AddServiceDefaults();
        // builder.Services.AddHostedService<Worker>();

        return builder;
    }

    private static TBuilder AddEndpointServices<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        builder.AddSqliteDbContext<ScribeContext>("scribe", configureDbContextOptions: dbContextOptionsBuilder =>
        {
            if (!builder.Environment.IsDevelopment())
            {
                return;
            }

            dbContextOptionsBuilder.EnableDetailedErrors();
            dbContextOptionsBuilder.EnableSensitiveDataLogging();
        });

        builder.Services
            .AddIdentityApiEndpoints<User>(ConfigureIdentity)
            .AddEntityFrameworkStores<ScribeContext>();

        builder.Services.AddAuthorization();

        return builder;
    }

    private static void ConfigureIdentity(IdentityOptions identityOptions)
    {
        identityOptions.Password.RequiredLength = 5;
        identityOptions.Password.RequireDigit = false;
        identityOptions.Password.RequireNonAlphanumeric = false;
        identityOptions.Password.RequireUppercase = false;
        identityOptions.Password.RequireLowercase = false;
    }


    public static WebApplication MapApplicationEndpoints(this WebApplication app)
    {
        app.UseRouting();
// app.UseRateLimiter();
// app.UseRequestLocalization();
// app.UseCors();

        app.UseAuthentication();
        app.UseAuthorization();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.MapDefaultEndpoints();
        app.MapIdentityEndpoint<User>();

        return app;
    }
}