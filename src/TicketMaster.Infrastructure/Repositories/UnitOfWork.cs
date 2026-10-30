using TicketMaster.Domain.Repositories;
using TicketMaster.Infrastructure.Context;
using System.Data;

namespace TicketMaster.Infrastructure.Repositories;

internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public async Task ExecuteInTransactionAsync(
        Func<Task> action,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(
            cancellationToken);

        try
        {
            await action();
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
