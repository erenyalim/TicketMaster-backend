using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TicketMaster.Domain.Cart;
using TicketMaster.Domain.Common;
using TicketMaster.Domain.Tickets;

namespace TicketMaster.Application.Cart;

public sealed record UpdateCartItemCommand(
    Guid CartItemId,
    int Quantity) : IRequest<Result<CartResponse>>;

public sealed class UpdateCartItemCommandValidator : AbstractValidator<UpdateCartItemCommand>
{
    public UpdateCartItemCommandValidator()
    {
        RuleFor(command => command.CartItemId)
            .NotEmpty().WithMessage("Cart item is required.");

        RuleFor(command => command.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
    }
}

internal sealed class UpdateCartItemCommandHandler(
    ICartRepository cartRepository,
    ITicketTypeRepository ticketTypeRepository,
    IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<UpdateCartItemCommand, Result<CartResponse>>
{
    public async Task<Result<CartResponse>> Handle(
        UpdateCartItemCommand request,
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

        IReadOnlyList<Domain.Cart.Cart> carts = await cartRepository.GetAllAsync(
            cart => (userId.HasValue && cart.UserId == userId) ||
                    (!userId.HasValue && cart.GuestToken == guestToken),
            cancellationToken);
        Domain.Cart.Cart? cart = carts.FirstOrDefault();
        CartItem? item = cart?.Items.FirstOrDefault(
            cartItem => cartItem.Id == request.CartItemId);

        if (cart is null || item is null)
        {
            return Result<CartResponse>.Failure("The specified cart item was not found.");
        }

        if (cart.ExpiresAt <= DateTimeOffset.UtcNow)
        {
            return Result<CartResponse>.Failure("The cart has expired.");
        }

        TicketType? ticketType = await ticketTypeRepository.GetByIdAsync(
            item.TicketTypeId,
            cancellationToken);
        if (ticketType is null)
        {
            return Result<CartResponse>.Failure("The ticket type for this cart item was not found.");
        }

        if (request.Quantity > ticketType.RemainingQuantity)
        {
            return Result<CartResponse>.Failure("The requested ticket quantity is not available.");
        }

        item.Quantity = request.Quantity;
        item.UnitPrice = ticketType.Price;
        await cartRepository.UpdateAsync(cart, cancellationToken);

        return Result<CartResponse>.Success(cart.ToResponse(guestToken));
    }
}
