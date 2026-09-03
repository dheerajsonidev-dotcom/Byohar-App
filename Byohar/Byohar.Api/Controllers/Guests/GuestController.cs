using Byohar.Application.Features.Guests.Commands.Create;
using Byohar.Application.Features.Guests.Commands.Update;
using Byohar.Application.Features.Guests.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Byohar.Api.Controllers.Guests;

[Route("api/[controller]")]
[ApiController]
[Microsoft.AspNetCore.Authorization.Authorize]
public class GuestController : BaseApiController<GuestController>
{
    [HttpPost("get-all")]
    public async Task<IActionResult> GetAll([FromBody] GetAllGuestQuery request)
    {
        return Ok(await _mediator.Send(request));
    }

    //[HttpGet("get-by-id/{id}")]
    //public async Task<IActionResult> GetById(Guid id)
    //{
    //    return Ok(await _mediator.Send(new GetGuestByIdQuery { Id = id }));
    //}

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateGuestCommand request)
    {
        return Ok(await _mediator.Send(request));
    }

    [HttpPost("update")]
    public async Task<IActionResult> Update([FromBody] UpdateGuestCommand request)
    {
        return Ok(await _mediator.Send(request));
    }

}
