using FluentValidation;
using Mapster;
using MediatR;
using System;
using TicketMaster.Domain.Tickets;
using static TicketMaster.Domain.Tickets.TicketType;

namespace TicketMaster.Application.Tickets;

public sealed record TicketTypeUpdateCommand(
    Guid Id,
    string Name,
    decimal Price,
    int TotalQuantity
) : IRequest<Result<string>>;

public sealed class TicketTypeUpdateCommandValidator : AbstractValidator<TicketTypeUpdateCommand>
{
    public TicketTypeUpdateCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Ticket ID is required.");
        RuleFor(x => x.Name)
            .NotEmpty().MaximumLength(100).WithMessage("Ticket type name is required.");
        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");
        RuleFor(x => x.TotalQuantity)
            .GreaterThan(0).WithMessage("Total quantity must be greater than zero.");
    }
}

internal sealed class TicketTypeUpdateCommandHandler(
    ITicketTypeRepository ticketRepository,
    TicketMaster.Domain.Events.IEventRepository eventRepository) : IRequestHandler<TicketTypeUpdateCommand, Result<string>>
{
    public async Task<Result<string>> Handle(
        TicketTypeUpdateCommand request,
        CancellationToken cancellationToken)
    {
        var ticketType = await ticketRepository.GetByIdAsync(request.Id, cancellationToken);

        if (ticketType is null)
        {
            return Result<string>.Failure("No ticket type was found to update.");
        }

        var ticketTypes = await ticketRepository.GetAllAsync(cancellationToken);
        if (ticketTypes.Any(t => t.Id != ticketType.Id &&
                                 t.EventId == ticketType.EventId &&
                                 t.Name == request.Name &&
                                 !t.IsDeleted))
        {
            return Result<string>.Failure("A ticket type with the specified name already exists for this event.");
        }

        var @event = await eventRepository.GetByIdAsync(ticketType.EventId, cancellationToken);
        if (@event is null)
        {
            return Result<string>.Failure("The specified event was not found.");
        }

        var otherQuantity = ticketTypes
            .Where(t => t.Id != ticketType.Id &&
                        t.EventId == ticketType.EventId &&
                        !t.IsDeleted)
            .Sum(t => t.TotalQuantity);

        if (otherQuantity + request.TotalQuantity > @event.Capacity)
        {
            return Result<string>.Failure("Ticket quantities cannot exceed event capacity.");
        }

        if (!ticketType.UpdateDetails(request.Name, request.Price, request.TotalQuantity))
        {
            return Result<string>.Failure("Total quantity cannot be lower than the number of tickets already sold.");
        }

        await ticketRepository.UpdateAsync(ticketType, cancellationToken);

        return Result<string>.Success("Ticket updated successfully.");
    }
}
