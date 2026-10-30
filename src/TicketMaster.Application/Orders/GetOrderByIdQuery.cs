using MediatR;
using TicketMaster.Application.Services;
using TicketMaster.Domain.Common;
using TicketMaster.Domain.Orders;

namespace TicketMaster.Application.Orders;

public sealed record GetOrderByIdQuery(Guid Id) : IRequest<Result<OrderResponse>>;

public sealed record OrderResponse(
    Guid Id,
    Guid? UserId,
    string? GuestEmail,
    decimal TotalAmount,
    string Currency,
    string Status,
    string PaymentStatus,
    IReadOnlyList<OrderItemResponse> Items);

public sealed record OrderItemResponse(
    Guid Id,
    Guid TicketTypeId,
    string TicketTypeName,
    decimal UnitPrice,
    int Quantity,
    decimal TotalAmount);

internal sealed class GetOrderByIdQueryHandler(
    IOrderRepository orderRepository,
    ICurrentUser currentUser)
    : IRequestHandler<GetOrderByIdQuery, Result<OrderResponse>>
{
    public async Task<Result<OrderResponse>> Handle(
        GetOrderByIdQuery request,
        CancellationToken cancellationToken)
    {
        Order? order = await orderRepository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (order is null)
        {
            return Result<OrderResponse>.Failure("The specified order was not found.");
        }

        bool ownsOrder = currentUser.IsAuthenticated
            ? order.UserId == currentUser.UserId
            : order.GuestToken == currentUser.GuestToken;

        if (!ownsOrder)
        {
            return Result<OrderResponse>.Failure("The specified order does not belong to the current user.");
        }

        List<OrderItemResponse> items = order.Items
            .Select(item => new OrderItemResponse(
                item.Id,
                item.TicketTypeId,
                item.TicketTypeName,
                item.UnitPrice,
                item.Quantity,
                item.TotalAmount))
            .ToList();

        return Result<OrderResponse>.Success(new OrderResponse(
            order.Id,
            order.UserId,
            order.GuestEmail,
            order.TotalAmount,
            order.Currency,
            order.Status.ToString(),
            order.PaymentStatus.ToString(),
            items));
    }
}
