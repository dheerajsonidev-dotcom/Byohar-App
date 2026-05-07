using Byohar.Application.Interfaces.Identity;
using Byohar.Application.Requests.Identity;
using Byohar.Shared.Wrapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;

namespace Byohar.Api.Controllers.Identity
{
    [Route("api/identity/token")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly ITokenService _identityService;
        //private readonly IHubContext<SignalRHub> _hubContext;
        private readonly ILogger<TokenController> _logger;

        public TokenController(ITokenService identityService, ILogger<TokenController> logger)
        {
            _identityService = identityService;
            _logger = logger;
        }

        /// <summary>
        /// Get Token (Email, Password)
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Status 200 OK</returns>
        [HttpPost]
        public async Task<ActionResult> Get(TokenRequest model)
        {
            var response = await _identityService.LoginAsync(model);
            return Ok(response);
        }

        /// <summary>
        /// Refresh Token
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Status 200 OK</returns>
        [HttpPost("refresh")]
        public async Task<ActionResult> Refresh([FromBody] RefreshTokenRequest model)
        {
            var response = await _identityService.GetRefreshTokenAsync(model);
            return Ok(response);
        }

        [HttpPost("removeRefereshToken")]
        public async Task<ActionResult> RemoveToken(RefreshTokenRequest model)
        {
            var response = await _identityService.RemoveRefereshToken(model);
            return Ok(response);
        }

        /// <summary>
        /// Remove login Device Info
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Status 200 OK</returns>
        [HttpPost("removeLoginDevice")]
        public async Task<ActionResult> RemoveLoginDevice([FromBody] RemoveLoginDeviceRequest model)
        {
            var response = await _identityService.RemoveLoginDevice(model);
            return Ok(response);
        }

        [HttpPost("GetTokenUsingWindows")]
        public async Task<ActionResult> GetTokenUsingWindows(UserDeviceInfo deviceInfo)
        {
            try
            {
                //First check if the Windows Authentication is already completed or Requested
                var result = await HttpContext.AuthenticateAsync("Windows");

                if (result?.Principal is WindowsPrincipal wp)
                {
                    _logger.LogInformation("Windows Authentication initiated");
                    //Get Current logon User Name
                    //var logonUser = WindowsIdentity.GetCurrent();
                    var logonUser = wp.Identity.Name;
                    _logger.LogInformation($"Current Windows User: {logonUser}");
                    _logger.LogInformation($"UserDomainName: {Environment.UserDomainName}");

                    //Get the Domain in which System is running
                    PrincipalContext context = new PrincipalContext(ContextType.Domain, Environment.UserDomainName);

                    if (context != null)
                    {
                        //Get User Details from the Active Directory
                        UserPrincipal user = UserPrincipal.FindByIdentity(context, logonUser);
                        if (user == null)
                        {
                            //if user not found Return UnAuthorized User
                            return Unauthorized();
                        }
                        var loginResult = await _identityService.LoginWithWindows(user, deviceInfo);
                        return Ok(loginResult);
                    }
                    else
                    {
                        return Ok(await Result.FailAsync("Domain Server Not Found!"));
                    }
                }
                else
                {
                    //Challenge Windows Authentication
                    return Challenge("Windows");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(await Result.FailAsync("Internal Server Error"));
            }
        }
        [HttpPost("GetTokenWithWindowsCredentials")]
        public async Task<ActionResult> GetTokenWithWindowsCredentials(TokenRequest request)
        {
            try
            {
                _logger.LogInformation("Windows Token request initiated");
                _logger.LogInformation($"UserDomainName: {Environment.UserDomainName}");
                PrincipalContext context = new PrincipalContext(ContextType.Domain, Environment.UserDomainName);
                if (context != null)
                {
                    //validate user with Active Directory
                    var isValid = context.ValidateCredentials(request.UserName, request.Password);
                    if (isValid)
                    {
                        //Get User Details from the Active Directory
                        UserPrincipal user = UserPrincipal.FindByIdentity(context, request.UserName);
                        if (user == null)
                        {
                            //if user not found Return UnAuthorized User
                            return Unauthorized();
                        }
                        var loginResult = await _identityService.LoginWithWindows(user, request.UserDeviceInfo);
                        return Ok(loginResult);
                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
                else
                {
                    return Ok(await Result.FailAsync("Domain Server Not Found!"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Ok(await Result.FailAsync("Internal Server Error"));
            }
        }
    }
}
