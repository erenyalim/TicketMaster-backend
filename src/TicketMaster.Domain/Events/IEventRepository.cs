using System;
using System.Collections.Generic;
using System.Text;
using TicketMaster.Domain.Repositories;

namespace TicketMaster.Domain.Events
{
    public interface IEventRepository : IRepository<Event>
    {
    }
}
