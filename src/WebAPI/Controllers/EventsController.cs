using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketMaster.Application.Auth;
using TicketMaster.Application.Events;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EventsController : ControllerBase
{
    private readonly IMediator _mediator;

    public EventsController(IMediator mediator)
    {
        _mediator = mediator;
    }


    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] EventGetAllQuery query)
    {
       
        var result = await _mediator.Send(query);

        if (result.IsSuccessful)
        {
            return Ok(result.Data);
        }

        return BadRequest(result.Message);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new EventGetByIdQuery(id));
        if (result.IsSuccessful)
        {
            return Ok(result.Data);
        }
        return NotFound(result.Message);
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> CreateEvent([FromBody] EventCreateCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsSuccessful)
        {
            return Ok(new
            {
                Message = result.Message,
                Id = result.Data
            });
        }

        return BadRequest(result.Message);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> DeleteEvent(Guid id)
    {
        var command = new EventDeleteCommand(id);
        var result = await _mediator.Send(command);

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


    [HttpPut("{id}")]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> UpdateEvent(Guid id, [FromBody] EventUpdateCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(new { Message = "The ID in the URL does not match the ID of the data you wish to update." });
        }

        var result = await _mediator.Send(command);

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
