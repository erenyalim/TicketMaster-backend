using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using TicketMaster.Application.DTOs;
using TicketMaster.Domain.Common;
using TicketMaster.Domain.Events;
using static TicketMaster.Domain.Events.Event;

namespace TicketMaster.Application.Events;

public sealed record EventCreateCommand(
    string Title,
    EventType Type,
    DateOnly Date,
    TimeOnly Time,
    string Description,
    int Capacity,
    AddressDto AddressDto
    ) : IRequest<Result<Guid>>;

public sealed class EventCreateCommandValidator : AbstractValidator<EventCreateCommand>
{
    public EventCreateCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");
        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid event type.");
        RuleFor(x => x.Date)
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today)).WithMessage("Event date must be today or in the future.");
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


internal sealed class EventCreateCommandHandler(
    IEventRepository eventRepository)
    : IRequestHandler<EventCreateCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(
        EventCreateCommand request,
        CancellationToken cancellationToken)
    {
        var isEventExists = await eventRepository.ExistsAsync(e =>
            e.Title == request.Title &&
            e.Date == request.Date,cancellationToken);
        if (isEventExists)
        {
            return Result<Guid>.Failure("Event with the specified title and date already exists.");
        }

        var address = new Address(
            request.AddressDto.Country,
            request.AddressDto.City,
            request.AddressDto.Location,
            request.AddressDto.FullAddress);

        var newEvent = new Event(
            request.Title,
            request.Type,
            request.Date,
            request.Time,
            request.Description,
            request.Capacity,
            address);

        await eventRepository.AddAsync(newEvent,cancellationToken);
        return Result<Guid>.Success(newEvent.Id, "Event created successfully.");
    }
}