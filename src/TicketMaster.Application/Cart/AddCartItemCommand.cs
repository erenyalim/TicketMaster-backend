using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TicketMaster.Domain.Cart;
using TicketMaster.Domain.Common;
using TicketMaster.Domain.Tickets;

namespace TicketMaster.Application.Cart;

public sealed record AddCartItemCommand(
    Guid TicketTypeId,
    int Quantity) : IRequest<Result<CartResponse>>;

public sealed class AddCartItemCommandValidator : AbstractValidator<AddCartItemCommand>
{
    public AddCartItemCommandValidator()
    {
        RuleFor(command => command.TicketTypeId)
            .NotEmpty().WithMessage("Ticket type is required.");

        RuleFor(command => command.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
    }
}

internal sealed class AddCartItemCommandHandler(
    ICartRepository cartRepository,
    ITicketTypeRepository ticketTypeRepository,
    IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<AddCartItemCommand, Result<CartResponse>>
{
    private static readonly TimeSpan CartLifetime = TimeSpan.FromDays(30);

    public async Task<Result<CartResponse>> Handle(
        AddCartItemCommand request,
        CancellationToken cancellationToken)
    {
        HttpContext? httpContext = httpContextAccessor.HttpContext;
        string? userIdValue = httpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        Guid? userId = Guid.TryParse(userIdValue, out Guid parsedUserId) ? parsedUserId : null;
        string? guestToken = httpContext?.Request.Headers["Guest-Token"].FirstOrDefault();

        if (userId is null && string.IsNullOrWhiteSpace(guestToken))
        {
            return Result<CartResponse>.Failure("An authenticated user or guest token is required.");
        }

        TicketType? ticketType = await ticketTypeRepository.GetByIdAsync(
            request.TicketTypeId,
            cancellationToken);
        if (ticketType is null)
        {
            return Result<CartResponse>.Failure("The specified ticket type was not found.");
        }

        IReadOnlyList<Domain.Cart.Cart> carts = await cartRepository.GetAllAsync(
            cart => (userId.HasValue && cart.UserId == userId) ||
                    (!userId.HasValue && cart.GuestToken == guestToken),
            cancellationToken);
        Domain.Cart.Cart? cart = carts.FirstOrDefault();

        bool isNewCart = cart is null;

        if (cart is null)
        {
            cart = new Domain.Cart.Cart(
                userId,
                guestToken,
                DateTimeOffset.UtcNow.Add(CartLifetime));
        }
        else if (cart.ExpiresAt <= DateTimeOffset.UtcNow)
        {
            cart.Items.Clear();
            cart.ExpiresAt = DateTimeOffset.UtcNow.Add(CartLifetime);
        }

        Domain.Cart.Cart cartEntity = cart;

        int currentQuantity = cartEntity.Items
            .Where(item => item.TicketTypeId == ticketType.Id)
            .Sum(item => item.Quantity);

        if (currentQuantity + request.Quantity > ticketType.RemainingQuantity)
        {
            return Result<CartResponse>.Failure("The requested ticket quantity is not available.");
        }

        CartItem? cartItem = cartEntity.Items.FirstOrDefault(
            item => item.TicketTypeId == ticketType.Id);

        if (cartItem is null)
        {
            cartEntity.Items.Add(new CartItem(
                cartEntity.Id,
                ticketType.Id,
                request.Quantity,
                ticketType.Price));
        }
        else
        {
            cartItem.Quantity += request.Quantity;
            cartItem.UnitPrice = ticketType.Price;
        }

        if (isNewCart)
        {
            await cartRepository.AddAsync(cartEntity, cancellationToken);
        }
        else
        {
            await cartRepository.UpdateAsync(cartEntity, cancellationToken);
        }

        return Result<CartResponse>.Success(cartEntity.ToResponse(guestToken));
    }
}
