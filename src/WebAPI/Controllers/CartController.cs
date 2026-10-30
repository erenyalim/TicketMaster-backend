using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketMaster.Application.Cart;
using TicketMaster.Domain.Common;

namespace WebAPI.Controllers;

[Route("api/cart")]
[ApiController]
[AllowAnonymous]
public sealed class CartController : ControllerBase
{
    private readonly IMediator _mediator;

    public CartController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetCart(CancellationToken cancellationToken)
    {
        Result<CartResponse> result = await _mediator.Send(
            new GetCartQuery(),
            cancellationToken);

        return result.IsSuccessful
            ? Ok(result.Data)
            : BadRequest(new { Message = result.Message });
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItem(
        [FromBody] AddCartItemCommand command,
        CancellationToken cancellationToken)
    {
        Result<CartResponse> result = await _mediator.Send(command, cancellationToken);

        return result.IsSuccessful
            ? Ok(result.Data)
            : BadRequest(new { Message = result.Message });
    }

    [HttpPatch("items/{id:guid}")]
    public async Task<IActionResult> UpdateItem(
        Guid id,
        [FromBody] UpdateCartItemCommand command,
        CancellationToken cancellationToken)
    {
        if (id != command.CartItemId)
        {
            return BadRequest(new { Message = "The ID in the URL does not match the cart item ID." });
        }

        Result<CartResponse> result = await _mediator.Send(command, cancellationToken);

        return result.IsSuccessful
            ? Ok(result.Data)
            : BadRequest(new { Message = result.Message });
    }

    [HttpDelete("items/{id:guid}")]
    public async Task<IActionResult> RemoveItem(
        Guid id,
        CancellationToken cancellationToken)
    {
        Result<CartResponse> result = await _mediator.Send(
            new RemoveCartItemCommand(id),
            cancellationToken);

        return result.IsSuccessful
            ? Ok(result.Data)
            : NotFound(new { Message = result.Message });
    }

    [HttpDelete]
    public async Task<IActionResult> ClearCart(CancellationToken cancellationToken)
    {
        Result<string> result = await _mediator.Send(
            new ClearCartCommand(),
            cancellationToken);

        return result.IsSuccessful
            ? Ok(new { Message = result.Data })
            : BadRequest(new { Message = result.Message });
    }
}
