using System.Reflection;
using System.Security.Claims;

using Api.Endpoints;

using Scribe.EntityFrameworkCore;
using Scribe.EntityFrameworkCore.Stores;

namespace Api;

public static class Extensions
{
    public static TBuilder AddApplicationServices<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        builder.Services.AddCors();
        builder.AddAuthServices();

        builder.AddSqliteDbContext<ScribeContext>("scribe");

        // Customizing run-time behavior during build-time document generation
        // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/aspnetcore-openapi#customizing-run-time-behavior-during-build-time-document-generation
        if (Assembly.GetEntryAssembly()?.GetName().Name == "GetDocument.Insider")
        {
            return builder;
        }

        builder.AddServiceDefaults();

        return builder;
    }

    private static TBuilder AddAuthServices<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddIdentityApiEndpoints<ScribeUser>(identityOptions =>
            {
                identityOptions.Stores.ProtectPersonalData = false;
            })
            .AddEntityFrameworkStores<ScribeContext>();

        builder.Services.AddAuthorizationBuilder()
            .AddPolicy("all_access", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(ClaimTypes.Role, "administrator");
            });

        return builder;
    }


    public static WebApplication MapApplicationEndpoints(this WebApplication app)
    {
        app.UseRouting();
        // app.UseRateLimiter();
        app.UseRequestLocalization();
        app.UseCors(corsBuilder => corsBuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

        app.UseAuthentication();
        app.UseAuthorization();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.MapDefaultEndpoints();

        app.MapGroup("/identity")
            .MapLoanApi()
            .MapIdentityApi<ScribeUser>();

        app.MapBookEndpoints();
        app.MapCategoryEndpoints();
        app.MapLoanApi();

        return app;
    }
}