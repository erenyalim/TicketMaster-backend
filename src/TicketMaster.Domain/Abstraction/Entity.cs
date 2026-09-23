using System;
using System.Collections.Generic;
using System.Text;
namespace TicketMaster.Domain.Abstraction;

public abstract class Entity : IAuditable, ISoftDeletable
{
    public Entity()
    {
        Id = Guid.CreateVersion7();
    }
    public Guid Id { get; protected set; }

    #region Audit Properties
    public System.DateTimeOffset CreatedAt { get; protected set; }
    public string? CreateUserId { get; protected set; }
    public System.DateTimeOffset? UpdatedAt { get; protected set; }
    public string? UpdatedUserId { get; protected set; }

    // Soft Delete Properties
    public bool IsDeleted { get; protected set; }
    public System.DateTimeOffset? DeletedAt { get; protected set; } 
    public string? DeletedUserId { get; protected set; }

    #endregion


}


