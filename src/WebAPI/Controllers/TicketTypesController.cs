using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketMaster.Application.Auth;
using TicketMaster.Application.Tickets;
namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TicketTypesController : ControllerBase
{
    private readonly IMediator _mediator;
    public TicketTypesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> CreateTicketType([FromBody] TicketTypeCreateCommand command)
    {
        var result = await _mediator.Send(command);
        if (result.IsSuccessful)
        {
            return Ok(new
            {
                Message = "Ticket type added successfully.",
                Id = result.Data
            });
        }
        return BadRequest(result.Message);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] TicketTypeGetAllQuery query)
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
        var result = await _mediator.Send(new TicketTypeGetByIdQuery(id));

        if (result.IsSuccessful)
        {
            return Ok(result.Data);
        }

        return NotFound(result.Message);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> UpdateTicketType(Guid id, [FromBody] TicketTypeUpdateCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(new
            {
                Message = "The ID in the URL does not match the ID of the data you wish to update."
            });
        }

        var result = await _mediator.Send(command);

        if (result.IsSuccessful)
        {
            return Ok(new { Message = result.Data });
        }

        return BadRequest(new { Message = result.Message });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> DeleteTicketType(Guid id)
    {
        var result = await _mediator.Send(new TicketTypeDeleteCommand(id));

        if (result.IsSuccessful)
        {
            return Ok(new { Message = result.Data });
        }

        return NotFound(new { Message = result.Message });
    }
}
