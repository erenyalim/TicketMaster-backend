using FluentValidation;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using TicketMaster.Application.DTOs;
using TicketMaster.Domain.Events;

namespace TicketMaster.Application.Events;

public sealed record EventUpdateCommand(
    Guid Id,
    string Title,
    Event.EventType Type,
    DateOnly Date,
    TimeOnly Time,
    string Description,
    int Capacity,
    AddressDto AddressDto
) : IRequest<Result<string>>;

public sealed class EventUpdateCommandValidator : AbstractValidator<EventUpdateCommand>
{
    public EventUpdateCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");
        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid event type.");
        RuleFor(x => x.Date)
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("Event date must be today or in the future.");
        RuleFor(x => x.Time)
            .NotEmpty().WithMessage("Event time is required.");
        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters.");
        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("Capacity must be greater than zero.");
        RuleFor(x => x.AddressDto)
            .NotNull().WithMessage("Address is required.");
    }
}
internal sealed class EventUpdateCommandHandler(IEventRepository eventRepository) : IRequestHandler<EventUpdateCommand, Result<string>>
{
    public async Task<Result<string>> Handle(
        EventUpdateCommand request,
        CancellationToken cancellationToken)
    {
        var @event = await eventRepository.GetByIdAsync(request.Id, cancellationToken);

        if (@event is null)
        {
            return Result<string>.Failure("No event was found to update.");
        }

        request.Adapt(@event);

        await eventRepository.UpdateAsync(@event, cancellationToken);

        return Result<string>.Success("Event updated successfully.");
    }
}