using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TicketMaster.Application.Services;

namespace TicketMaster.Infrastructure.Services;

internal sealed class CurrentUser : ICurrentUser
{
    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        string? userIdValue = httpContextAccessor.HttpContext?
            .User
            .FindFirstValue(ClaimTypes.NameIdentifier);

        if (Guid.TryParse(userIdValue, out Guid userId))
        {
            UserId = userId;
            IsAuthenticated = true;
        }

        GuestToken = httpContextAccessor.HttpContext?
            .Request
            .Headers["Guest-Token"]
            .FirstOrDefault();
    }

    public Guid? UserId { get; }
    public string? GuestToken { get; }
    public bool IsAuthenticated { get; }
}
