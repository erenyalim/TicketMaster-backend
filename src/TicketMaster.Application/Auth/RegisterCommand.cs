using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using TicketMaster.Domain.Users;

namespace TicketMaster.Application.Auth;

public sealed record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string? Phone
    ) : IRequest<Result<string>>;

public sealed class RegisterCommandHandle(UserManager<AppUser> userManager) : IRequestHandler<RegisterCommand, Result<string>>
{

    public async Task<Result<string>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var isEmailExists = await userManager.FindByEmailAsync(request.Email);
        if (isEmailExists is not null)
        {
            return Result<string>.Failure("Email already exists.");
        }

        var user = new AppUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserName = request.Email,
            PhoneNumber = request.Phone

        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (result.Succeeded)
        {
            var roleResult = await userManager.AddToRoleAsync(user, RoleNames.User);

            if (!roleResult.Succeeded)
            {
                await userManager.DeleteAsync(user);

                return Result<string>.Failure(
                    roleResult.Errors.First().Description);
            }

            return Result<string>.Success("User successfully registered!");
        }

        return Result<string>.Failure(result.Errors.First().Description);
    }
}
