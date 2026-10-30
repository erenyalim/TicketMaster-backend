using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TicketMaster.Domain.Cart;
using TicketMaster.Domain.Common;

namespace TicketMaster.Application.Cart;

public sealed record ClearCartCommand : IRequest<Result<string>>;

internal sealed class ClearCartCommandHandler(
    ICartRepository cartRepository,
    IHttpContextAccessor httpContextAccessor)
    : IRequestHandler<ClearCartCommand, Result<string>>
{
    public async Task<Result<string>> Handle(
        ClearCartCommand request,
        CancellationToken cancellationToken)
    {
        HttpContext? httpContext = httpContextAccessor.HttpContext;
        string? userIdValue = httpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        Guid? userId = Guid.TryParse(userIdValue, out Guid parsedUserId) ? parsedUserId : null;
        string? guestToken = httpContext?.Request.Headers["Guest-Token"].FirstOrDefault();

        if (userId is null && string.IsNullOrWhiteSpace(guestToken))
        {
            return Result<string>.Failure("An authenticated user or guest token is required.");
        }

        IReadOnlyList<Domain.Cart.Cart> carts = await cartRepository.GetAllAsync(
            cart => (userId.HasValue && cart.UserId == userId) ||
                    (!userId.HasValue && cart.GuestToken == guestToken),
            cancellationToken);
        Domain.Cart.Cart? cart = carts.FirstOrDefault();

        if (cart is null)
        {
            return Result<string>.Success("Cart is already empty.");
        }

        cart.Items.Clear();
        await cartRepository.UpdateAsync(cart, cancellationToken);
        return Result<string>.Success("Cart cleared successfully.");
    }
}
