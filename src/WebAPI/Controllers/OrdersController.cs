using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketMaster.Application.Orders;
using TicketMaster.Domain.Common;

namespace WebAPI.Controllers;

[Route("api/orders")]
[ApiController]
public sealed class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout(
        [FromBody] CreateOrderCommand command,
        CancellationToken cancellationToken)
    {
        Result<Guid> result = await _mediator.Send(command, cancellationToken);

        if (result.IsSuccessful)
        {
            return Ok(new { OrderId = result.Data });
        }

        return BadRequest(new { Message = result.Message });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        Result<OrderResponse> result = await _mediator.Send(
            new GetOrderByIdQuery(id),
            cancellationToken);

        if (result.IsSuccessful)
        {
            return Ok(result.Data);
        }

        return NotFound(new { Message = result.Message });
    }

    [HttpPost("{id:guid}/payment-callback")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize]
    public async Task<IActionResult> PaymentCallback(
        Guid id,
        [FromBody] PaymentCallbackRequest request,
        CancellationToken cancellationToken)
    {
        Result result = await _mediator.Send(
            new PaymentCallbackCommand(id, request.IsSuccessful, request.PaymentReference),
            cancellationToken);

        if (result.IsSuccessful)
        {
            return NoContent();
        }

        return BadRequest(new { Message = result.Message });
    }
}

public sealed record PaymentCallbackRequest(bool IsSuccessful, string? PaymentReference);
