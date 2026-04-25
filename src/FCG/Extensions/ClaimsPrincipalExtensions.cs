using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FCG.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var sub = user.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (sub is null || !Guid.TryParse(sub, out var id))
            throw new InvalidOperationException("Token sem identificacao de usuario.");
        return id;
    }
}
