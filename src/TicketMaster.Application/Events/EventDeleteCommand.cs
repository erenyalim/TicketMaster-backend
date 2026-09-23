using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using TicketMaster.Domain.Events;

namespace TicketMaster.Application.Events;

public sealed record EventDeleteCommand(Guid Id) : IRequest<Result<string>>;

internal sealed class EventDeleteCommandHandler(IEventRepository eventRepository) : IRequestHandler<EventDeleteCommand, Result<string>>
{
    public async Task<Result<string>> Handle(
        EventDeleteCommand request,
        CancellationToken cancellationToken)
    {
        var @event = await eventRepository.GetByIdAsync(request.Id, cancellationToken);

        if (@event is null)
        {
            return Result<string>.Failure("No event was found to delete.");
        }

        await eventRepository.DeleteAsync(@event, cancellationToken);

        return Result<string>.Success("The event has been successfully deleted from the system.");
    }
}