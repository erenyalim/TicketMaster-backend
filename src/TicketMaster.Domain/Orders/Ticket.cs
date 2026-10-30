using TicketMaster.Domain.Abstraction;

namespace TicketMaster.Domain.Orders;

public sealed class Ticket : Entity
{
    public Ticket(
        Guid orderId,
        Guid ticketTypeId,
        Guid? userId,
        string ticketCode,
        string qrCodeValue,
        string? holderEmail)
    {
        OrderId = orderId;
        TicketTypeId = ticketTypeId;
        UserId = userId;
        TicketCode = ticketCode;
        QrCodeValue = qrCodeValue;
        HolderEmail = holderEmail;
        Status = TicketStatus.Active;
    }

    public enum TicketStatus { Active, Used, Cancelled }

    public Guid OrderId { get; set; }
    public Guid TicketTypeId { get; set; }
    public Guid? UserId { get; set; }
    public string TicketCode { get; set; } = string.Empty;
    public string QrCodeValue { get; set; } = string.Empty;
    public string? HolderEmail { get; set; }
    public TicketStatus Status { get; set; }
    public DateTimeOffset? UsedAt { get; set; }
}