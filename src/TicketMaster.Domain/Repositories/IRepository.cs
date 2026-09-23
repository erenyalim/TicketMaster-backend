using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using TicketMaster.Domain.Abstraction;

namespace TicketMaster.Domain.Repositories;

public interface IRepository<T> where T : Entity
{
    Task AddAsync(
        T entity,
        CancellationToken cancellationToken = default);

    Task<T?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        T entity,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        T entity,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> GetAllAsync(
        CancellationToken cancellationToken = default);
}

