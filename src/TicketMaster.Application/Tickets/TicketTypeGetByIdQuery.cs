using Mapster;
using MediatR;
using System;
using TicketMaster.Domain.Tickets;

namespace TicketMaster.Application.Tickets;

public sealed record TicketTypeGetByIdQuery(Guid Id) : IRequest<Result<TicketTypeGetByIdQueryResponse>>;

public sealed record TicketTypeGetByIdQueryResponse(
    Guid Id,
    string Name,
    decimal Price,
    int TotalQuantity,
    Guid EventId
);

internal sealed class TicketTypeGetByIdQueryHandler(ITicketTypeRepository ticketRepository) : IRequestHandler<TicketTypeGetByIdQuery, Result<TicketTypeGetByIdQueryResponse>>
{
    public async Task<Result<TicketTypeGetByIdQueryResponse>> Handle(
        TicketTypeGetByIdQuery request,
        CancellationToken cancellationToken)
    {
        var ticket = await ticketRepository.GetByIdAsync(request.Id, cancellationToken);

        if (ticket is null)
        {
            return Result<TicketTypeGetByIdQueryResponse>.Failure("Could not find ticket that matches the provided ID.");
        }

        var response = ticket.Adapt<TicketTypeGetByIdQueryResponse>();
        return Result<TicketTypeGetByIdQueryResponse>.Success(response);
    }
}
