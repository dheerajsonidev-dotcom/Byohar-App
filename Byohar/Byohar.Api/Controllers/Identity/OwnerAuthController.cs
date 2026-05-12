using Byohar.Application.Interfaces.Identity;
using Byohar.Application.Requests.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Byohar.Api.Controllers.Identity;

[Route("api/[controller]")]
[ApiController]
public class OwnerAuthController : ControllerBase
{
    private readonly IOwnerAuthService _ownerAuthService;

    public OwnerAuthController(IOwnerAuthService ownerAuthService)
    {
        _ownerAuthService = ownerAuthService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(OwnerRegisterRequest request)
    {
        var response = await _ownerAuthService.RegisterOwnerAsync(request);

        return Ok(response);
    }
}
