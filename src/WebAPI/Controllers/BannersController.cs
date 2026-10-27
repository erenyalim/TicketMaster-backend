using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketMaster.Application.Auth;
using TicketMaster.Application.Banners;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class BannersController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] BannerGetAllQuery query)
    {
        var result = await mediator.Send(query);

        if (result.IsSuccessful)
        {
            return Ok(result.Data);
        }

        return BadRequest(result.Message);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await mediator.Send(new BannerGetByIdQuery(id));

        if (result.IsSuccessful)
        {
            return Ok(result.Data);
        }

        return NotFound(result.Message);
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> Create([FromBody] BannerCreateCommand command)
    {
        var result = await mediator.Send(command);

        if (result.IsSuccessful)
        {
            return Ok(new
            {
                Message = result.Message,
                Id = result.Data
            });
        }

        return BadRequest(new
        {
            Message = result.Message,
            Errors = result.ValidationErrors
        });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> Update(Guid id, [FromBody] BannerUpdateCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(new { Message = "The ID in the URL does not match the ID of the data you wish to update." });
        }

        var result = await mediator.Send(command);

        if (result.IsSuccessful)
        {
            return Ok(new { Message = result.Data });
        }

        return BadRequest(new
        {
            Message = result.Message,
            Errors = result.ValidationErrors
        });
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await mediator.Send(new BannerDeleteCommand(id));

        if (result.IsSuccessful)
        {
            return Ok(new { Message = result.Data });
        }

        return BadRequest(new
        {
            Message = result.Message,
            Errors = result.ValidationErrors
        });
    }
}
