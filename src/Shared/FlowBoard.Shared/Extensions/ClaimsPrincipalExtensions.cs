using System.Security.Claims;

namespace FlowBoard.Shared.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirst(ClaimTypes.NameIdentifier)
                 ?? user.FindFirst("sub");
        if (claim == null || !int.TryParse(claim.Value, out var id))
            throw new UnauthorizedAccessException("User ID not found in token.");
        return id;
    }

    public static string GetEmail(this ClaimsPrincipal user)
        => user.FindFirst(ClaimTypes.Email)?.Value
        ?? user.FindFirst("email")?.Value
        ?? string.Empty;

    public static string GetRole(this ClaimsPrincipal user)
        => user.FindFirst(ClaimTypes.Role)?.Value
        ?? user.FindFirst("role")?.Value
        ?? string.Empty;

    public static bool IsAdmin(this ClaimsPrincipal user)
        => user.GetRole() == "PlatformAdmin";
}