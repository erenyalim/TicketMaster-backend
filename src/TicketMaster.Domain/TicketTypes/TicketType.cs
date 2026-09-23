using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using TicketMaster.Domain.Abstraction;
using TicketMaster.Domain.Events;
using static TicketMaster.Domain.Tickets.TicketType;

namespace TicketMaster.Domain.Tickets;


public sealed class TicketType : Entity
{

    public TicketType(
        Guid eventId,
        string name,
        decimal price,
        int totalQuantity)
    {
        EventId = eventId;
        Name = name;
        Price = price;
        TotalQuantity = totalQuantity;
        RemainingQuantity = totalQuantity;
    }

    public bool UpdateDetails(string name, decimal price, int totalQuantity)
    {
        var soldQuantity = TotalQuantity - RemainingQuantity;
        if (totalQuantity < soldQuantity)
        {
            return false;
        }

        Name = name;
        Price = price;
        TotalQuantity = totalQuantity;
        RemainingQuantity = totalQuantity - soldQuantity;
        return true;
    }

    public Guid EventId { get; private set; }
    public string Name { get; private set; } = default!;
    public decimal Price { get; private set; }
    public int TotalQuantity { get; private set; }
    public int RemainingQuantity { get; private set; }
}
