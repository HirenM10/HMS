using Asp.Versioning;
using Clinical.API.Authentication;
using Clinical.Application.Abstractions.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clinical.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[AllowAnonymous]
[Route("api/v{version:apiVersion}/auth")]
public sealed class AuthController(
    IUserAuthenticationService authenticationService,
    ITokenService tokenService) : ControllerBase
{
    [HttpPost("login")]
    [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await authenticationService.AuthenticateAsync(request.Email, request.Password, cancellationToken);
        if (user is null)
        {
            return Unauthorized();
        }

        return Ok(new LoginResponse(
            tokenService.CreateToken(user),
            "Bearer",
            user.Role,
            user.Email,
            user.FullName));
    }
}
