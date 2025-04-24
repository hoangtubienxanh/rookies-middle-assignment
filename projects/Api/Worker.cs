using System.Security.Claims;

using Microsoft.AspNetCore.Identity;

using Scribe;
using Scribe.AspNetCore.Identity.Extensions.Stores;

// ReSharper disable MethodSupportsCancellation

namespace Api;

public class Worker(ILogger<Worker> logger, IHostApplicationLifetime lifetime, IServiceScopeFactory serviceScopeFactory)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken _)
    {
        // ReSharper disable once InlineTemporaryVariable
        var stoppingToken = _;
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = serviceScopeFactory.CreateScope();
            await using var db = scope.ServiceProvider.GetRequiredService<ScribeContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var userStore = scope.ServiceProvider.GetRequiredService<IUserStore<User>>();
            var emailStore = (IUserEmailStore<User>)userStore;

            await using var transaction = await db.Database.BeginTransactionAsync(CancellationToken.None);
            var cancellationSource = new CancellationTokenSource();
            var cancellationToken = cancellationSource.Token;
            try
            {
                var powerUser = new User();
                await userStore.SetUserNameAsync(powerUser, "teams@scire.app", cancellationToken);
                await emailStore.SetEmailAsync(powerUser, "teams@scire.app", cancellationToken);
                await userManager.CreateAsync(powerUser, "$$$myVery0wnPassword");

                var normalUser = new User();
                await userStore.SetUserNameAsync(normalUser, "demo@scire.app", cancellationToken);
                await emailStore.SetEmailAsync(normalUser, "demo@scire.app", cancellationToken);
                await userManager.CreateAsync(normalUser, "$$$myVery0wnPassword");

                var powerUserRole = new Claim(ClaimTypes.Role, "SUPER");
                await userManager.AddClaimAsync(powerUser, powerUserRole);

                var borrower = new Borrower() { UserId = new Guid(normalUser.Id) };
                
                var borrowingRequest = new BorrowingRequestState()
                {
                    Borrower = borrower,
                    Approver = powerUser,
                    DateProcessed = DateTimeOffset.Now,
                    DateRequested = DateTimeOffset.Now,
                    
                    
                }
                db.Requests.Add()

                // Commit transaction if all commands succeed, the transaction will auto-rollback
                // when disposed if either commands fails
                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                // ignored
            }


            await Task.Delay(1000, cancellationToken);
            lifetime.StopApplication();
        }
    }
}