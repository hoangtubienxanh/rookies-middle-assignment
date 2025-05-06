using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;

using Scribe.EntityFrameworkCore.Stores;

namespace Api.Services;

public class SameLoanApplicantHandler : AuthorizationHandler<SameLoanApplicantRequirement, LoanApplication>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        SameLoanApplicantRequirement requirement,
        LoanApplication resource)
    {
        var userId = Guid.Parse(context.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (userId == resource.ApplicantId)
        {
            context.Succeed(requirement);
        }
    }
}