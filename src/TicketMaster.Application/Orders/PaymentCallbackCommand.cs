using MediatR;
using TicketMaster.Application.Services;
using TicketMaster.Domain.Common;
using TicketMaster.Domain.Orders;

namespace TicketMaster.Application.Orders;

public sealed record PaymentCallbackCommand(
    Guid OrderId,
    bool IsSuccessful,
    string? PaymentReference) : IRequest<Result>;

internal sealed class PaymentCallbackCommandHandler(
    IOrderRepository orderRepository,
    ICurrentUser currentUser)
    : IRequestHandler<PaymentCallbackCommand, Result>
{
    public async Task<Result> Handle(
        PaymentCallbackCommand request,
        CancellationToken cancellationToken)
    {
        Order? order = await orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
        {
            return Result.Failure("The specified order was not found.");
        }

        if (!currentUser.IsAuthenticated || order.UserId != currentUser.UserId)
        {
            return Result.Failure("Only the authenticated order owner can confirm payment.");
        }

        if (order.Status != Order.OrderStatus.Pending ||
            order.PaymentStatus != Order.PaymentState.Pending)
        {
            return Result.Failure("The order is not awaiting payment.");
        }

        if (request.IsSuccessful)
        {
            order.PaymentStatus = Order.PaymentState.Paid;
            order.Status = Order.OrderStatus.Completed;
            order.PaymentReference = request.PaymentReference;
        }
        else
        {
            order.PaymentStatus = Order.PaymentState.Failed;
            order.Status = Order.OrderStatus.Cancelled;
        }

        await orderRepository.UpdateAsync(order, cancellationToken);
        return Result.Success();
    }
}
