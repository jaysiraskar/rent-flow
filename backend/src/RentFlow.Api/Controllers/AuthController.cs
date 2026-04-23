using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentFlow.Api.Extensions;
using RentFlow.Application.DTOs.Auth;
using RentFlow.Application.Interfaces;

namespace RentFlow.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
        => Ok(await authService.RegisterAsync(request, cancellationToken));

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
        => Ok(await authService.LoginAsync(request, cancellationToken));

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<AuthResponse>> Me(CancellationToken cancellationToken)
    {
        AuthResponse? me = await authService.GetMeAsync(HttpContext.GetUserId(), cancellationToken);
        return me is null ? NotFound() : Ok(me);
    }
}
