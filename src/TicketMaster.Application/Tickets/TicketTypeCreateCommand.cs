using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using TicketMaster.Domain.Tickets;
using TicketMaster.Domain.Events;

namespace TicketMaster.Application.Tickets;

public sealed record TicketTypeCreateCommand
(
 string Name,
 decimal Price,
 Guid EventId,
 int TotalQuantity

    ) : IRequest<Result<Guid>>;

public sealed class TicketTypeCreateCommandValidator : AbstractValidator<TicketTypeCreateCommand>
{
    public TicketTypeCreateCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().MaximumLength(100).WithMessage("Ticket type name is required.");
        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");
        RuleFor(x => x.EventId)
            .NotEmpty().WithMessage("EventId is required.");
        RuleFor(x => x.TotalQuantity)
            .GreaterThan(0).WithMessage("Total quantity must be greater than zero.");
    }

    internal sealed class TicketTypeCreateCommandHandler(
        ITicketTypeRepository ticketRepository,
        IEventRepository eventRepository) : IRequestHandler<TicketTypeCreateCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(
            TicketTypeCreateCommand request,
            CancellationToken cancellationToken)
        {
            var @event = await eventRepository.GetByIdAsync(request.EventId, cancellationToken);
            if (@event is null)
            {
                return Result<Guid>.Failure("The specified event was not found.");
            }

            var ticketTypes = await ticketRepository.GetAllAsync(cancellationToken);
            if (ticketTypes.Any(t => t.EventId == request.EventId &&
                                     t.Name == request.Name &&
                                     !t.IsDeleted))
            {
                return Result<Guid>.Failure("A ticket type with the specified name already exists for this event.");
            }

            var usedCapacity = ticketTypes
                .Where(t => t.EventId == request.EventId && !t.IsDeleted)
                .Sum(t => t.TotalQuantity);

            if (usedCapacity + request.TotalQuantity > @event.Capacity)
            {
                return Result<Guid>.Failure("Ticket quantities cannot exceed event capacity.");
            }

            var ticket = new TicketType(
                request.EventId,
                request.Name,
                request.Price,
                request.TotalQuantity);
            await ticketRepository.AddAsync(ticket, cancellationToken);
            return Result<Guid>.Success(ticket.Id);
        }
    }
}
