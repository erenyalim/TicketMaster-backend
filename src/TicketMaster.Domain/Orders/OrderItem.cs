using TicketMaster.Domain.Abstraction;

namespace TicketMaster.Domain.Orders;

public sealed class OrderItem : Entity
{
    public OrderItem(
        Guid orderId,
        Guid ticketTypeId,
        string ticketTypeName,
        decimal unitPrice,
        int quantity)
    {
        OrderId = orderId;
        TicketTypeId = ticketTypeId;
        TicketTypeName = ticketTypeName;
        UnitPrice = unitPrice;
        Quantity = quantity;
        TotalAmount = unitPrice * quantity;
    }

    public Guid OrderId { get; set; }
    public Guid TicketTypeId { get; set; }
    public string TicketTypeName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalAmount { get; set; }

    public Order? Order { get; set; }
}