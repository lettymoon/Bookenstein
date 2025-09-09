using Bookenstein.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookenstein.Application.Interfaces
{
    public interface ITokenService
    {
        (string accessToken, DateTime expiresAtUtc, string jti) CreateAccessToken(User user);
        (string refreshTokenPlain, string refreshTokenHash, DateTime expiresAtUtc) CreateRefreshToken();
    }
}
