using System;
using System.Collections.Generic;
using System.Text;
using TicketMaster.Domain.Repositories;

namespace TicketMaster.Domain.Tickets;

public interface ITicketTypeRepository : IRepository<TicketType>
{
    Task<IReadOnlyList<TicketType>> GetByEventIdAsync(
        Guid eventId,
        CancellationToken cancellationToken = default);
}
