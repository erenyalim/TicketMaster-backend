using System;
using System.Collections.Generic;
using System.Text;
using TicketMaster.Domain.Repositories;
using static TicketMaster.Domain.Events.Event;

namespace TicketMaster.Domain.Events
{
    public interface IEventRepository : IRepository<Event>
    {
        Task<IReadOnlyList<Event>> GetFilteredAsync(
            EventType? category,
            string? search,
            CancellationToken cancellationToken = default);
    }
}
