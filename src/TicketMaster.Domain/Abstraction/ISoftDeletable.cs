using System;

namespace TicketMaster.Domain.Abstraction;

public interface ISoftDeletable
{
    bool IsDeleted { get; }
    DateTimeOffset? DeletedAt { get; }
    string? DeletedUserId { get; }
}
