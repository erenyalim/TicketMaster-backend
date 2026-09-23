using MediatR;
using Mapster;
using System;
using System.Collections.Generic;
using System.Text;
using TicketMaster.Application.DTOs;
using TicketMaster.Domain.Events;
using static TicketMaster.Domain.Events.Event;

namespace TicketMaster.Application.Events;

public sealed record EventGetByIdQuery(Guid Id) : IRequest<Result<EventGetByIdQueryResponse>>; 
public sealed record EventGetByIdQueryResponse(
        Guid Id,
        string Title,
        EventType Type,
        DateOnly Date,
        TimeOnly Time,
        string Description,
        int Capacity,
        AddressDto Address
    );

internal sealed class EventGetByIdQueryHandler(IEventRepository eventRepository) : IRequestHandler<EventGetByIdQuery, Result<EventGetByIdQueryResponse>>
{
    public async Task<Result<EventGetByIdQueryResponse>> Handle(
        EventGetByIdQuery request,
        CancellationToken cancellationToken)
    {
        var @event = await eventRepository.GetByIdAsync(request.Id);

        if (@event is null)
        {
            return Result<EventGetByIdQueryResponse>.Failure("Could not find event that matches the provided ID.");
        }

        var response = @event.Adapt<EventGetByIdQueryResponse>();
        return Result<EventGetByIdQueryResponse>.Success(response);
    }
}