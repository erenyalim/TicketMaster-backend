using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using TicketMaster.Application.DTOs;
using TicketMaster.Domain.Tickets;

namespace TicketMaster.Application.Tickets;

public sealed class TicketTypeGetAllQuery() : IRequest<Result<List<TicketTypeGetAllQueryResponse>>>;

public sealed record TicketTypeGetAllQueryResponse(
 Guid Id,
 string Name,
 decimal Price,
 Guid EventId
);

internal sealed class TicketTypeGetAllQueryHandler(ITicketTypeRepository ticketRepository) : IRequestHandler<TicketTypeGetAllQuery, Result<List<TicketTypeGetAllQueryResponse>>>
{
    public async Task<Result<List<TicketTypeGetAllQueryResponse>>> Handle(
        TicketTypeGetAllQuery request,
        CancellationToken cancellationToken)
    {
        var tickets = await ticketRepository.GetAllAsync(cancellationToken);

        var response = tickets.Adapt<List<TicketTypeGetAllQueryResponse>>();

        return Result<List<TicketTypeGetAllQueryResponse>>.Success(response);
    }
}