using TicketMaster.Domain.Orders;
using TicketMaster.Infrastructure.Context;

namespace TicketMaster.Infrastructure.Repositories;

internal sealed class TicketRepository : Repository<Ticket>, ITicketRepository
{
    public TicketRepository(AppDbContext context) : base(context)
    {
    }
}
