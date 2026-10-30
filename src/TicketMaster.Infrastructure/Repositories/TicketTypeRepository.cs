using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TicketMaster.Domain.Events;
using TicketMaster.Domain.Tickets;
using TicketMaster.Infrastructure.Context;
using TicketMaster.Infrastructure.Repositories;

namespace TicketMaster.Infrastructure.Repositories;

internal sealed class TicketTypeRepository : Repository<TicketType>, ITicketTypeRepository
{
    public TicketTypeRepository(AppDbContext context) : base(context)
    {

    }

    public async Task<IReadOnlyList<TicketType>> GetByEventIdAsync(
        Guid eventId,
        CancellationToken cancellationToken = default)
    {
        return await Context.TicketTypes
            .AsNoTracking()
            .Where(ticketType => ticketType.EventId == eventId)
            .ToListAsync(cancellationToken);
    }
}

