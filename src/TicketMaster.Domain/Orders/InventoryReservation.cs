using TicketMaster.Domain.Abstraction;

namespace TicketMaster.Domain.Orders;

public sealed class InventoryReservation : Entity
{
    public InventoryReservation(
        Guid orderId,
        Guid ticketTypeId,
        int quantity,
        DateTimeOffset expiresAt)
    {
        OrderId = orderId;
        TicketTypeId = ticketTypeId;
        Quantity = quantity;
        ExpiresAt = expiresAt;
        Status = ReservationStatus.Locked;
    }

    public enum ReservationStatus { Locked, Released, Completed, Expired }

    public Guid OrderId { get; set; }
    public Guid TicketTypeId { get; set; }
    public int Quantity { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public ReservationStatus Status { get; set; }
}