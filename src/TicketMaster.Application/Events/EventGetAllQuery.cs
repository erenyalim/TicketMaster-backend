using MediatR;
using System;
using Mapster;
using System.Collections.Generic;
using System.Text;
using TicketMaster.Application.DTOs;
using TicketMaster.Application.Services;
using TicketMaster.Domain.Abstraction;
using TicketMaster.Domain.Common;
using TicketMaster.Domain.Events;
using static TicketMaster.Domain.Events.Event;

namespace TicketMaster.Application.Events;

public sealed record EventGetAllQuery() : IRequest<Result<List<EventGetAllQueryResponse>>>;

public sealed record EventGetAllQueryResponse(
    Guid Id,
    string Title,
    EventType Type,
    DateOnly Date,
    TimeOnly Time,
    string Description,
    int Capacity,
    AddressDto Address,
    string ImageUrl
);

internal sealed class EventGetAllQueryHandler(
    IEventRepository eventRepository,
    IBlobStorageService blobStorageService) : IRequestHandler<EventGetAllQuery, Result<List<EventGetAllQueryResponse>>>
{
    public async Task<Result<List<EventGetAllQueryResponse>>> Handle(
        EventGetAllQuery request,
        CancellationToken cancellationToken)
    {
        var events = await eventRepository.GetAllAsync(cancellationToken);

        var response = events.Adapt<List<EventGetAllQueryResponse>>();

        for (var i = 0; i < response.Count; i++)
        {
            response[i] = response[i] with
            {
                ImageUrl = blobStorageService.NormalizeUrl(response[i].ImageUrl)
            };
        }

        return Result<List<EventGetAllQueryResponse>>.Success(response);
    }
}





