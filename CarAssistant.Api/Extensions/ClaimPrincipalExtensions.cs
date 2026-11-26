using System.Security.Claims;

namespace CarAssistant.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var idString = user.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? throw new InvalidOperationException("User Id claim missing");
        return Guid.Parse(idString);
    }
}
