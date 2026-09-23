using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Scrutor;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using TicketMaster.Application.Auth;
using TicketMaster.Domain.Events;
using TicketMaster.Domain.Tickets;
using TicketMaster.Domain.Users;
using TicketMaster.Infrastructure.Context;
using TicketMaster.Infrastructure.Repositories;
using TicketMaster.Infrastructure.Options;
using TicketMaster.Application.Services;
using TicketMaster.Infrastructure.Services;

namespace TicketMaster.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options => { 

        string connectionString = configuration.GetConnectionString("SqlServer")!;
        options.UseSqlServer(connectionString);

    });

        services.AddDataProtection();
        services.AddHttpContextAccessor();


        //User Manager
        services
            .AddIdentityCore<AppUser>(opt =>
            {
                opt.Password.RequiredLength = 1;
                opt.Password.RequireDigit = false;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireNonAlphanumeric = false;

                opt.Lockout.MaxFailedAccessAttempts = 5;
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

                opt.SignIn.RequireConfirmedEmail = false;
            })

            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders()
            .AddSignInManager();


        services.Configure<JwtOptions>(configuration.GetSection("JwtOptions"));

        JwtOptions jwtOptions = configuration
            .GetSection("JwtOptions")
            .Get<JwtOptions>()
            ?? throw new InvalidOperationException("JwtOptions configuration is missing.");

        if (string.IsNullOrWhiteSpace(jwtOptions.SecretKey))
        {
            throw new InvalidOperationException("JwtOptions:SecretKey configuration is missing.");
        }

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
                    RoleClaimType = ClaimTypes.Role,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization();


        services.AddScoped<ITicketTypeRepository, TicketTypeRepository>();
        services.AddScoped<IEventRepository, EventRepository>();
        
        services.Scan(opt => opt
            
            .FromAssemblies(typeof(DependencyInjection).Assembly)
            .AddClasses(publicOnly: false)
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsImplementedInterfaces()
            .WithScopedLifetime()

            );

        return services;
    }

}
