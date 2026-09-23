using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TicketMaster.Application.Services;
using TicketMaster.Domain.Users;
using TicketMaster.Infrastructure.Options;

namespace TicketMaster.Infrastructure.Services;

internal sealed class JwtProvider (
    IOptions<JwtOptions> options,
    UserManager<AppUser> userManager) : IJwtProvider
{
    public async Task<string> CreateTokenAsync(AppUser user, CancellationToken cancellationToken = default)
    {
        IList<string> roles = await userManager.GetRolesAsync(user);

        List<Claim> claims = new List<Claim>
        {
               new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        };

        foreach (string role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var expires = DateTime.UtcNow.AddHours(1);

        SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.SecretKey));
        SigningCredentials signingCredentials = new(securityKey, SecurityAlgorithms.HmacSha512);

       

        JwtSecurityToken securityToken = new JwtSecurityToken(
            issuer: options.Value.Issuer,
            audience: options.Value.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expires,
            signingCredentials: signingCredentials
        );

        JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
        
        string token = handler.WriteToken(securityToken);

        return token;
    }
}
