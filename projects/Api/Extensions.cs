using System.Reflection;
using System.Security.Claims;

using Api.Endpoints;
using Api.Services;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

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
        builder.Services.AddProblemDetails();

        builder.AddSqliteDbContext<ScribeContext>("scribe");
        builder.Services.AddScoped<IBookManager, BookManager>();
        builder.Services.AddScoped<ICategoryManager, CategoryManager>();
        builder.Services.AddScoped<ILoanApplicationManager, LoanApplicationManager>();

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

        builder.Services.AddSingleton<IAuthorizationHandler, SameLoanApplicantHandler>();

        builder.Services.AddAuthorizationBuilder()
            .AddPolicy("applicant_creator", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.Requirements.Add(new SameLoanApplicantRequirement());
            })
            .AddPolicy("all_access", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(ClaimTypes.Role, "administrator");
            });

        builder.Services.AddScoped<CurrentUser>();
        builder.Services.AddScoped<IClaimsTransformation, ClaimsTransformation>();
        return builder;
    }


    public static WebApplication MapApplicationEndpoints(this WebApplication app)
    {
        app.UseExceptionHandler();
        app.UseStatusCodePages();

        app.UseRouting();
        // app.UseRateLimiter();
        // app.UseRequestLocalization();
        app.UseCors(corsBuilder => corsBuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

        app.UseAuthentication();
        app.UseAuthorization();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.MapDefaultEndpoints();

        app.MapGroup("/identity")
            .MapIdentityApi<ScribeUser>();

        app.MapBookEndpoints();
        app.MapCategoryEndpoints();
        app.MapLoanApi();

        return app;
    }
}