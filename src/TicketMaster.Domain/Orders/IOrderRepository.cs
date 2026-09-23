using System;
using System.Collections.Generic;
using System.Text;
using TicketMaster.Domain.Repositories;

namespace TicketMaster.Domain.Orders
{
    public interface IOrderRepository : IRepository<Order>
    {
    }
}
