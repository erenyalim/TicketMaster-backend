using TicketMaster.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using TicketMaster.Infrastructure.Context;

namespace TicketMaster.Infrastructure.Repositories;

internal sealed class BannerRepository : Repository<Banner>, IBannerRepository
{
    public BannerRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Banner>> GetActiveOrderedAsync(
        CancellationToken cancellationToken = default)
    {
        return await Context.Banners
            .AsNoTracking()
            .Where(banner => banner.IsActive)
            .OrderBy(banner => banner.DisplayOrder)
            .ThenBy(banner => banner.Title)
            .ToListAsync(cancellationToken);
    }
}
