using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TicketMaster.Domain.Cart;
using TicketMaster.Domain.Common;

namespace TicketMaster.Application.Cart;

public sealed record CartResponse(
    Guid? Id,
    Guid? UserId,
    string? GuestToken,
    decimal TotalAmount,
    IReadOnlyList<CartItemResponse> Items);

public sealed record CartItemResponse(
    Guid Id,
    Guid TicketTypeId,
    string TicketTypeName,
    Guid EventId,
    int Quantity,
    decimal UnitPrice,
    decimal TotalAmount);

public sealed record GetCartQuery : IRequest<Result<CartResponse>>;

internal sealed class GetCartQueryHandler(
    ICartRepository cartRepository,
    IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<GetCartQuery, Result<CartResponse>>
{
    public async Task<Result<CartResponse>> Handle(
        GetCartQuery request,
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

        if (cart is null)
        {
            return Result<CartResponse>.Success(new CartResponse(
                null,
                userId,
                guestToken,
                0,
                Array.Empty<CartItemResponse>()));
        }

        if (cart.ExpiresAt <= DateTimeOffset.UtcNow)
        {
            return Result<CartResponse>.Success(new CartResponse(
                cart.Id,
                cart.UserId,
                guestToken,
                0,
                Array.Empty<CartItemResponse>()));
        }

        return Result<CartResponse>.Success(cart.ToResponse(guestToken));
    }
}

internal static class CartResponseMapper
{
    public static CartResponse ToResponse(this Domain.Cart.Cart cart, string? guestToken = null)
    {
        List<CartItemResponse> items = cart.Items
            .Select(item => new CartItemResponse(
                item.Id,
                item.TicketTypeId,
                item.TicketType?.Name ?? string.Empty,
                item.TicketType?.EventId ?? Guid.Empty,
                item.Quantity,
                item.UnitPrice,
                item.UnitPrice * item.Quantity))
            .ToList();

        return new CartResponse(
            cart.Id,
            cart.UserId,
            guestToken ?? cart.GuestToken,
            items.Sum(item => item.TotalAmount),
            items);
    }
}
