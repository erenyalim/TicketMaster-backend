using TicketMaster.Domain.Repositories;

namespace TicketMaster.Domain.Entities;

public interface IBannerRepository : IRepository<Banner>
{
    Task<IReadOnlyList<Banner>> GetActiveOrderedAsync(
        CancellationToken cancellationToken = default);
}
