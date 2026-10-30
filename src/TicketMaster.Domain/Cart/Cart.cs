using TicketMaster.Domain.Abstraction;

namespace TicketMaster.Domain.Cart;

public sealed class Cart : Entity
{
    public Cart(Guid? userId, string? guestToken, DateTimeOffset expiresAt)
    {
        UserId = userId;
        GuestToken = guestToken;
        ExpiresAt = expiresAt;
    }

    public Guid? UserId { get; set; }
    public string? GuestToken { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }

    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}