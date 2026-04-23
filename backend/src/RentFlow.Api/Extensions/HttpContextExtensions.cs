using System.Security.Claims;

namespace RentFlow.Api.Extensions;

public static class HttpContextExtensions
{
    public static Guid GetUserId(this HttpContext context)
    {
        string? value = context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? context.User.FindFirstValue("sub");
        return Guid.TryParse(value, out Guid id) ? id : Guid.Empty;
    }
}
