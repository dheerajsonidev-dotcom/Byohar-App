using Byohar.Application.Features.Events.Commands.Create;
using Byohar.Application.Features.Events.Commands.Update;
using Byohar.Application.Features.Events.Queries.GetAll;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Byohar.Api.Controllers.Events;

[Route("api/[controller]")]
[ApiController]
public class EventController : BaseApiController<EventController>
{
  

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateEventCommand command)
    {
        return Ok(await _mediator.Send(command));
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> Update( UpdateEventCommand command)
    {
        return Ok(await _mediator.Send(command));
    }

    [HttpPost("get-all")]
    public async Task<IActionResult> GetAll([FromQuery] GetAllEventQuery query)
    {
        return Ok(await _mediator.Send(query));
    }
}
