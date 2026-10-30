using TicketMaster.Domain.Orders;
using TicketMaster.Infrastructure.Context;

namespace TicketMaster.Infrastructure.Repositories;

internal sealed class InventoryReservationRepository : Repository<InventoryReservation>, IInventoryReservationRepository
{
    public InventoryReservationRepository(AppDbContext context) : base(context)
    {
    }
}
