namespace TicketMaster.Application.Services;

public interface ICurrentUser
{
    Guid? UserId { get; }
    string? GuestToken { get; }
    bool IsAuthenticated { get; }
}
