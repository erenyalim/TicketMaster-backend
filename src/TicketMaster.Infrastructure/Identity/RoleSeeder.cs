using Microsoft.AspNetCore.Identity;
using TicketMaster.Application.Auth;

namespace TicketMaster.Infrastructure.Identity;

public static class RoleSeeder
{
    public static async Task SeedAsync(RoleManager<IdentityRole<Guid>> roleManager)
    {
        string[] roles =
        [
            RoleNames.Admin,
            RoleNames.User
        ];

        foreach (string role in roles)
        {
            if (await roleManager.RoleExistsAsync(role))
            {
                continue;
            }

            IdentityResult result = await roleManager.CreateAsync(
                new IdentityRole<Guid>(role));

            if (!result.Succeeded)
            {
                string errors = string.Join(", ", result.Errors.Select(error => error.Description));

                throw new InvalidOperationException(
                    $"The '{role}' role could not be created: {errors}");
            }
        }
    }
}
