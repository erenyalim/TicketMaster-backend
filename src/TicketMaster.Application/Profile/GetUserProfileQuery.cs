using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using MediatR;
using Microsoft.AspNetCore.Identity;
using TicketMaster.Domain.Abstraction;
using TicketMaster.Domain.Users;

namespace TicketMaster.Application.Profile;

public sealed record UserProfileResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    string ProfilePictureUrl
);

public sealed record GetUserProfileQuery : IRequest<Result<UserProfileResponse>>;

internal sealed class GetUserProfileQueryHandler(
    UserManager<AppUser> userManager,
    IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<GetUserProfileQuery, Result<UserProfileResponse>>
{
    public async Task<Result<UserProfileResponse>> Handle(
        GetUserProfileQuery request,
        CancellationToken cancellationToken)
    {
        var userIdValue = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdValue, out var userId))
        {
            return Result<UserProfileResponse>.Failure("The authenticated user could not be identified.");
        }

        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return Result<UserProfileResponse>.Failure("User not found.");
        }

        return Result<UserProfileResponse>.Success(user.ToProfileResponse());
    }
}

internal static class ProfileMapper
{
    public static UserProfileResponse ToProfileResponse(this AppUser user)
    {
        return new UserProfileResponse(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email ?? string.Empty,
            user.PhoneNumber,
            user.ProfilePictureUrl);
    }
}
