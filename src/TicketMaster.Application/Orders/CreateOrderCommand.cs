using FluentValidation;
using MediatR;
using TicketMaster.Application.Services;
using TicketMaster.Domain.Cart;
using TicketMaster.Domain.Common;
using TicketMaster.Domain.Orders;
using TicketMaster.Domain.Repositories;
using TicketMaster.Domain.Tickets;

namespace TicketMaster.Application.Orders;

public sealed record CreateOrderCommand(
    Guid CartId,
    string? GuestFirstName,
    string? GuestLastName,
    string? GuestEmail) : IRequest<Result<Guid>>;

public sealed class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(command => command.CartId)
            .NotEmpty().WithMessage("Cart ID is required.");

        RuleFor(command => command.GuestFirstName)
            .NotEmpty()
            .When(command =>
                !string.IsNullOrWhiteSpace(command.GuestLastName) ||
                !string.IsNullOrWhiteSpace(command.GuestEmail))
            .WithMessage("Guest first name is required when guest information is provided.");

        RuleFor(command => command.GuestLastName)
            .NotEmpty()
            .When(command =>
                !string.IsNullOrWhiteSpace(command.GuestFirstName) ||
                !string.IsNullOrWhiteSpace(command.GuestEmail))
            .WithMessage("Guest last name is required when guest information is provided.");

        RuleFor(command => command.GuestEmail)
            .NotEmpty()
            .EmailAddress()
            .When(command =>
                !string.IsNullOrWhiteSpace(command.GuestFirstName) ||
                !string.IsNullOrWhiteSpace(command.GuestLastName))
            .WithMessage("A valid guest email is required when guest information is provided.");
    }
}

internal sealed class CreateOrderCommandHandler(
    ICartRepository cartRepository,
    ITicketTypeRepository ticketTypeRepository,
    IOrderRepository orderRepository,
    IInventoryReservationRepository reservationRepository,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser)
    : IRequestHandler<CreateOrderCommand, Result<Guid>>
{
    private static readonly TimeSpan ReservationLifetime = TimeSpan.FromMinutes(15);

    public async Task<Result<Guid>> Handle(
        CreateOrderCommand request,
        CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated && string.IsNullOrWhiteSpace(currentUser.GuestToken))
        {
            return Result<Guid>.Failure("An authenticated user or guest token is required.");
        }

        if (!currentUser.IsAuthenticated &&
            (string.IsNullOrWhiteSpace(request.GuestFirstName) ||
             string.IsNullOrWhiteSpace(request.GuestLastName) ||
             string.IsNullOrWhiteSpace(request.GuestEmail)))
        {
            return Result<Guid>.Failure("Guest first name, last name, and email are required.");
        }

        Domain.Cart.Cart? cart = await cartRepository.GetByIdAsync(request.CartId, cancellationToken);
        if (cart is null)
        {
            return Result<Guid>.Failure("The specified cart was not found.");
        }

        bool ownsCart = currentUser.IsAuthenticated
            ? cart.UserId == currentUser.UserId
            : cart.GuestToken == currentUser.GuestToken;

        if (!ownsCart)
        {
            return Result<Guid>.Failure("The specified cart does not belong to the current user.");
        }

        if (cart.ExpiresAt <= DateTimeOffset.UtcNow)
        {
            return Result<Guid>.Failure("The cart has expired.");
        }

        if (cart.Items.Count == 0)
        {
            return Result<Guid>.Failure("The cart is empty.");
        }

        Guid orderId = Guid.Empty;

        try
        {
            await unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                List<(CartItem Item, TicketType TicketType)> items = new();
                decimal totalAmount = 0;

                foreach (CartItem cartItem in cart.Items)
                {
                    TicketType? ticketType = await ticketTypeRepository.GetByIdAsync(
                        cartItem.TicketTypeId,
                        cancellationToken);

                    if (ticketType is null)
                    {
                        throw new InvalidOperationException("A ticket type in the cart was not found.");
                    }

                    IReadOnlyList<InventoryReservation> reservations =
                        await reservationRepository.GetAllAsync(
                            reservation => reservation.TicketTypeId == ticketType.Id &&
                                           reservation.Status == InventoryReservation.ReservationStatus.Locked &&
                                           reservation.ExpiresAt > DateTimeOffset.UtcNow,
                            cancellationToken);

                    int reservedQuantity = reservations.Sum(reservation => reservation.Quantity);
                    if (ticketType.RemainingQuantity - reservedQuantity < cartItem.Quantity)
                    {
                        throw new InvalidOperationException(
                            $"Insufficient stock for ticket type '{ticketType.Name}'.");
                    }

                    totalAmount += ticketType.Price * cartItem.Quantity;
                    items.Add((cartItem, ticketType));
                }

                Order order = new(
                    currentUser.UserId,
                    currentUser.IsAuthenticated ? null : currentUser.GuestToken,
                    cart.Id,
                    totalAmount,
                    "TRY",
                    currentUser.IsAuthenticated ? null : request.GuestFirstName,
                    currentUser.IsAuthenticated ? null : request.GuestLastName,
                    currentUser.IsAuthenticated ? null : request.GuestEmail);

                foreach ((CartItem cartItem, TicketType ticketType) in items)
                {
                    order.Items.Add(new OrderItem(
                        order.Id,
                        ticketType.Id,
                        ticketType.Name,
                        ticketType.Price,
                        cartItem.Quantity));
                }

                await orderRepository.AddAsync(order, cancellationToken);

                foreach ((CartItem cartItem, TicketType ticketType) in items)
                {
                    await reservationRepository.AddAsync(
                        new InventoryReservation(
                            order.Id,
                            ticketType.Id,
                            cartItem.Quantity,
                            DateTimeOffset.UtcNow.Add(ReservationLifetime)),
                        cancellationToken);
                }

                orderId = order.Id;
            }, cancellationToken);
        }
        catch (InvalidOperationException exception)
        {
            return Result<Guid>.Failure(exception.Message);
        }

        return Result<Guid>.Success(orderId);
    }
}
