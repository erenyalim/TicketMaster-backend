using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketMaster.Application.Profile;

namespace WebAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class UserController(IMediator mediator) : ControllerBase
{
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetUserProfileQuery(), cancellationToken);

        if (result.IsSuccessful)
        {
            return Ok(result.Data);
        }

        return result.Message == "User not found."
            ? NotFound(result.Message)
            : Unauthorized(result.Message);
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile(
        [FromBody] UpdateUserProfileCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsSuccessful)
        {
            return Ok(result.Data);
        }

        return result.Message == "User not found."
            ? NotFound(result.Message)
            : BadRequest(new
            {
                Message = result.Message,
                Errors = result.ValidationErrors
            });
    }
}
