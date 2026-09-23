using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketMaster.Application.Auth;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class AuthController(IMediator mediator) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsSuccessful)
        {
            return StatusCode(StatusCodes.Status201Created, new
            {
                Message = result.Data
            });
        }

        if (result.Message == "Email already exists.")
        {
            return Conflict(new { Message = result.Message });
        }

        return BadRequest(new
        {
            Message = result.Message,
            Errors = result.ValidationErrors
        });
    }
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsSuccessful)
        {
            return Ok(result.Data);
        }

        return Unauthorized(new
        {
            Message = result.Message,
            Errors = result.ValidationErrors
        });
    }
}
