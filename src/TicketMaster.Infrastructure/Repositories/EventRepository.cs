using Microsoft.EntityFrameworkCore;
using TicketMaster.Domain.Events;
using TicketMaster.Infrastructure.Context;

namespace TicketMaster.Infrastructure.Repositories;

internal sealed class EventRepository : Repository<Event>, IEventRepository
{
    public EventRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Event>> GetFilteredAsync(
        Event.EventType? category,
        string? search,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Events.AsNoTracking();

        if (category.HasValue)
        {
            query = query.Where(@event => @event.Type == category.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchTerm = search.Trim();
            query = query.Where(@event =>
                EF.Functions.Like(@event.Title, $"%{searchTerm}%"));
        }

        return await query.ToListAsync(cancellationToken);
    }
}
