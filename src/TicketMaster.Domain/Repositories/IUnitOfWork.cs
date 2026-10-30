namespace TicketMaster.Domain.Repositories;

public interface IUnitOfWork
{
    Task ExecuteInTransactionAsync(
        Func<Task> action,
        CancellationToken cancellationToken = default);
}
