using TicketMaster.Domain.Abstraction;

namespace TicketMaster.Domain.Orders;

public sealed class Order : Entity
{
    public Order(
        Guid? userId,
        string? guestToken,
        Guid cartId,
        decimal totalAmount,
        string currency,
        string? guestFirstName,
        string? guestLastName,
        string? guestEmail)
    {
        UserId = userId;
        GuestToken = guestToken;
        CartId = cartId;
        TotalAmount = totalAmount;
        Currency = currency;
        GuestFirstName = guestFirstName;
        GuestLastName = guestLastName;
        GuestEmail = guestEmail;
        Status = OrderStatus.Pending;
        PaymentStatus = PaymentState.Pending;
    }

    public enum OrderStatus { Pending, Completed, Cancelled, Refunded }
    public enum PaymentState { Pending, Paid, Failed, Refunded }

    public Guid? UserId { get; set; }
    public string? GuestToken { get; set; }
    public Guid CartId { get; set; }
    public string? GuestFirstName { get; set; }
    public string? GuestLastName { get; set; }
    public string? GuestEmail { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = "TRY";
    public OrderStatus Status { get; set; }
    public PaymentState PaymentStatus { get; set; }
    public string? PaymentReference { get; set; }

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
