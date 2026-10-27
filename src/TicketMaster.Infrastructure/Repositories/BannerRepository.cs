using TicketMaster.Domain.Entities;
using TicketMaster.Infrastructure.Context;

namespace TicketMaster.Infrastructure.Repositories;

internal sealed class BannerRepository : Repository<Banner>, IBannerRepository
{
    public BannerRepository(AppDbContext context) : base(context)
    {
    }
}
