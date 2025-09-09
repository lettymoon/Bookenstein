using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

[ApiController]
[Route("api/[controller]")]
public class DebugController : ControllerBase
{
    [HttpPost("dump-token")]
    public IActionResult DumpToken([FromBody] string token)
    {
        var h = new JwtSecurityTokenHandler();
        if (!h.CanReadToken(token)) return BadRequest("Token malformado");
        var t = h.ReadJwtToken(token);

        return Ok(new
        {
            Header = t.Header.ToDictionary(k => k.Key, v => v.Value?.ToString()),
            Issuer = t.Issuer,
            Audiences = t.Audiences,
            ValidToUtc = t.ValidTo,
            Claims = t.Claims.Select(c => new { c.Type, c.Value })
        });
    }
}
