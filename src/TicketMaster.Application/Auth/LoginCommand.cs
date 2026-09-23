using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TicketMaster.Application.Services;
using TicketMaster.Domain.Users;

namespace TicketMaster.Application.Auth;

public sealed record LoginCommand
(
    string Email,
    string Password
) : IRequest<Result<LoginCommandResponse>>;

public sealed record LoginCommandResponse
(
    string AccessToken
);

public sealed class LoginCommandHandler (
    UserManager<AppUser> userManager,
    SignInManager<AppUser> signInManager,
    IJwtProvider jwtProvider
    ) : IRequestHandler<LoginCommand, Result<LoginCommandResponse>>
{
    public async Task<Result<LoginCommandResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        AppUser? user = await userManager.Users.FirstOrDefaultAsync(
            p => p.Email == request.Email,
            cancellationToken);
        
        if (user is null)
        {
            return Result<LoginCommandResponse>.Failure("User not found.");
        }
        
       SignInResult signInResult = await signInManager.CheckPasswordSignInAsync(user, request.Password, true);

        if (signInResult.IsLockedOut)
        {
            TimeSpan? timeSpan = user.LockoutEnd - DateTime.UtcNow;
            if (timeSpan is not null)
                return Result<LoginCommandResponse>.Failure(
                    $"You are locked out for {Math.Ceiling(timeSpan.Value.TotalMinutes)} minutes");
            else
                return Result<LoginCommandResponse>.Failure(
                    "You are temporarily locked out due to multiple failed login attempts.");
        }

        if (signInResult.IsNotAllowed)
        {
            return Result<LoginCommandResponse>.Failure("Your email address is not confirmed.");
        }

        if (!signInResult.Succeeded)
        {
            return Result<LoginCommandResponse>.Failure("Your password is incorrect.");
        }

        var token = await jwtProvider.CreateTokenAsync(user, cancellationToken);

        var response = new LoginCommandResponse(token);

        return Result<LoginCommandResponse>.Success(response);
    }
}