using System;
using System.Collections.Generic;
using System.Text;
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
}

