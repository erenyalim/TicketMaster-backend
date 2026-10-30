using TicketMaster.Domain.Orders;
using TicketMaster.Infrastructure.Context;

namespace TicketMaster.Infrastructure.Repositories;

internal sealed class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(AppDbContext context) : base(context)
    {
    }
}
