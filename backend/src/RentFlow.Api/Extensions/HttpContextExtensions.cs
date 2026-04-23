using System.Security.Claims;

namespace RentFlow.Api.Extensions;

public static class HttpContextExtensions
{
    public static Guid GetUserId(this HttpContext context)
    {
        var value = context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? context.User.FindFirstValue("sub");
        if (!Guid.TryParse(value, out var id) || id == Guid.Empty)
            throw new UnauthorizedAccessException("Invalid authentication token.");

        return id;
    }
}
