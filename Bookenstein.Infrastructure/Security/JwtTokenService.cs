using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Bookenstein.Application.Interfaces;
using Bookenstein.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Bookenstein.Infrastructure.Security;

public sealed class JwtOptions
{
    public string Issuer { get; set; } = "Bookenstein";
    public string Audience { get; set; } = "Bookenstein";
    public string Key { get; set; } = default!; // chave HS256 (>= 32 chars)
    public int AccessTokenMinutes { get; set; } = 15;
    public int RefreshTokenDays { get; set; } = 7;
}

public sealed class JwtTokenService : ITokenService
{
    private readonly JwtOptions _opt;
    private readonly byte[] _keyBytes;

    public JwtTokenService(IOptions<JwtOptions> options)
    {
        _opt = options.Value;
        _keyBytes = Encoding.UTF8.GetBytes(_opt.Key);
    }

    public (string accessToken, DateTime expiresAtUtc, string jti) CreateAccessToken(User user)
    {
        // garante que sempre haverá um SecurityStamp no token
        if (string.IsNullOrWhiteSpace(user.SecurityStamp))
        {
            user.SecurityStamp = Guid.NewGuid().ToString("N");
        }
        if (user.PasswordUpdatedAt == default)
            user.PasswordUpdatedAt = DateTime.UtcNow;

        var now = DateTime.UtcNow;
        var exp = now.AddMinutes(_opt.AccessTokenMinutes);
        var jti = Guid.NewGuid().ToString("N");

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, jti),
            new Claim(JwtRegisteredClaimNames.Iat, ((DateTimeOffset)now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),

            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),

            // versão da senha (ticks) e stamp para invalidar tokens antigos
            new Claim("pwdv", user.PasswordUpdatedAt.Ticks.ToString()),
            new Claim("sstamp", user.SecurityStamp),
        };

        var creds = new SigningCredentials(new SymmetricSecurityKey(_keyBytes), SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _opt.Issuer,
            audience: _opt.Audience,
            claims: claims,
            notBefore: now,
            expires: exp,
            signingCredentials: creds);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return (jwt, exp, jti);
    }

    public (string refreshTokenPlain, string refreshTokenHash, DateTime expiresAtUtc) CreateRefreshToken()
    {
        var plain = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)); // 512-bit
        var hash = Sha256(plain);
        var exp = DateTime.UtcNow.AddDays(_opt.RefreshTokenDays);
        return (plain, hash, exp);
    }

    private static string Sha256(string input)
    {
        using var sha = SHA256.Create();
        return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(input)));
    }
}
