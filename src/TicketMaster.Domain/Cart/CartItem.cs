using TicketMaster.Domain.Abstraction;
using TicketMaster.Domain.Tickets;

namespace TicketMaster.Domain.Cart;

public sealed class CartItem : Entity
{
    public CartItem(Guid cartId, Guid ticketTypeId, int quantity, decimal unitPrice)
    {
        CartId = cartId;
        TicketTypeId = ticketTypeId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public Guid CartId { get; set; }
    public Guid TicketTypeId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    public Cart? Cart { get; set; }
    public TicketType? TicketType { get; set; }
}
