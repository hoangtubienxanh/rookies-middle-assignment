using System.Security.Claims;

using Scribe.EntityFrameworkCore.Stores;

namespace Api.Services;

public class CurrentUser
{
    public ScribeUser? User { get; set; }
    public ClaimsPrincipal Principal { get; set; } = null!;
    public Guid Id => Guid.Parse(Principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
    public bool IsAdmin => Principal.IsInRole("administrator");
}