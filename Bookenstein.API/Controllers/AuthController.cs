using Bookenstein.Application.Contracts;
using Bookenstein.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Bookenstein.API.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    public AuthController(IAuthService auth) => _auth = auth;

    [HttpPost("signup")]
    [EnableRateLimiting("auth")]
    public async Task<ActionResult<AuthResponse>> Signup([FromBody] SignupBody body, CancellationToken ct)
    {
        var email = body.Email.Trim().ToLowerInvariant();

        var newUser = new SignupRequest(
            Name: body.Name,
            Username: email,
            Email: email,
            Password: body.Password
        );

        var response = await _auth.SignupAsync(newUser, ct);
        return Created("", response);
    }

    [HttpPost("login")]
    [EnableRateLimiting("auth")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var response = await _auth.LoginAsync(request, ct);
        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh([FromBody] RefreshRequest request, CancellationToken ct)
    {
        var response = await _auth.RefreshAsync(request, ct);
        return Ok(response);
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken ct)
    {
        var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                  ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(sub, out var userId)) return Unauthorized();

        await _auth.ChangePasswordAsync(userId, request, ct);
        return NoContent();
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshRequest request, CancellationToken ct)
    {
        await _auth.LogoutAsync(request, ct);
        return NoContent();
    }

    [HttpGet("ping")]
    [Authorize]
    public IActionResult Ping() => Ok(new { ok = true, name = User.Identity?.Name });

}
