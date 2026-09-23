namespace TicketMaster.Domain.Abstraction;

public interface IAuditable
{
    DateTimeOffset CreatedAt { get; }
    string? CreateUserId { get; }
    DateTimeOffset? UpdatedAt { get; }
    string? UpdatedUserId { get; }
    bool IsDeleted { get; }
    DateTimeOffset? DeletedAt { get; }
    string? DeletedUserId { get; }

}
