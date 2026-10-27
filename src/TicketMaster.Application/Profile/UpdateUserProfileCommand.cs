using System.Security.Claims;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using TicketMaster.Domain.Abstraction;
using TicketMaster.Domain.Users;

namespace TicketMaster.Application.Profile;

public sealed record UpdateUserProfileCommand(
    string FirstName,
    string LastName,
    string? Phone,
    string? ProfilePictureUrl = null
) : IRequest<Result<UserProfileResponse>>;

public sealed class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(50);
        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(50);
        RuleFor(x => x.Phone)
            .MaximumLength(30);
        RuleFor(x => x.ProfilePictureUrl)
            .MaximumLength(2048);
    }
}

internal sealed class UpdateUserProfileCommandHandler(
    UserManager<AppUser> userManager,
    IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<UpdateUserProfileCommand, Result<UserProfileResponse>>
{
    public async Task<Result<UserProfileResponse>> Handle(
        UpdateUserProfileCommand request,
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

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.PhoneNumber = request.Phone;

        if (request.ProfilePictureUrl is not null)
        {
            user.ProfilePictureUrl = request.ProfilePictureUrl;
        }

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return Result<UserProfileResponse>.Failure(result.Errors.First().Description);
        }

        return Result<UserProfileResponse>.Success(user.ToProfileResponse());
    }
}
