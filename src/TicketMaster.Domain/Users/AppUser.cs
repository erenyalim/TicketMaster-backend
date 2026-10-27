using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using TicketMaster.Domain.Abstraction;

namespace TicketMaster.Domain.Users;

public sealed class AppUser : IdentityUser<Guid>, IAuditable
{
    public AppUser()
    {
        Id = Guid.CreateVersion7();
    }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string ProfilePictureUrl { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";

    #region Audit Properties
    public System.DateTimeOffset CreatedAt { get; private set; }
    public string? CreateUserId { get; private set; }
    public System.DateTimeOffset? UpdatedAt { get; private set; }
    public string? UpdatedUserId { get; private set; }
    public bool IsDeleted { get; private set; }
    public System.DateTimeOffset? DeletedAt { get; private set; }
    public string? DeletedUserId { get; private set; }

    #endregion  
}
