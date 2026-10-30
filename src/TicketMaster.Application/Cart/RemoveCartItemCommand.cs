using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TicketMaster.Domain.Cart;
using TicketMaster.Domain.Common;

namespace TicketMaster.Application.Cart;

public sealed record RemoveCartItemCommand(Guid CartItemId) : IRequest<Result<CartResponse>>;

public sealed class RemoveCartItemCommandValidator : AbstractValidator<RemoveCartItemCommand>
{
    public RemoveCartItemCommandValidator()
    {
        RuleFor(command => command.CartItemId)
            .NotEmpty().WithMessage("Cart item is required.");
    }
}

internal sealed class RemoveCartItemCommandHandler(
    ICartRepository cartRepository,
    IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<RemoveCartItemCommand, Result<CartResponse>>
{
    public async Task<Result<CartResponse>> Handle(
        RemoveCartItemCommand request,
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

        cart.Items.Remove(item);
        await cartRepository.UpdateAsync(cart, cancellationToken);
        return Result<CartResponse>.Success(cart.ToResponse(guestToken));
    }
}
