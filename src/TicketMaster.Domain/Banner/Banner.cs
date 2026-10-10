using TicketMaster.Domain.Abstraction;
using TicketMaster.Domain.Events;

namespace TicketMaster.Domain.Entities;

public class Banner : Entity
{
    public string Title { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;

    public Guid? EventId { get; set; }
    public Event? Event { get; set; }

    public int DisplayOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
}