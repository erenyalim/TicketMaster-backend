using TicketMaster.Domain.Common;

namespace TicketMaster.Application.Services;

public sealed record PaymentRequest(
    Guid OrderId,
    decimal Amount,
    string Currency,
    string? BuyerEmail);

public interface IPaymentService
{
    Task<Result<string>> ProcessPaymentAsync(
        PaymentRequest request,
        CancellationToken cancellationToken = default);
}
