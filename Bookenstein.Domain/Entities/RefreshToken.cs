using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookenstein.Domain.Entities
{
    public sealed class RefreshToken : Entity
        
    {
        public Guid UserId { get; private set; }
        public string TokenHash { get; private set; } = default!;
        public DateTime ExpiresAt { get; private set; }
        public DateTime? RevokedAt { get; private set; }
        public string? ReplacedByTokenHash { get; private set; }

        private RefreshToken() { }
        public RefreshToken(Guid userId, string tokenHash, DateTime expiresAt)
        {
            UserId = userId; TokenHash = tokenHash; ExpiresAt = expiresAt;
        }

        public bool IsActive => RevokedAt is null && DateTime.UtcNow < ExpiresAt;
        public void Revoke(string? replacedByTokenHash = null)
        {
            RevokedAt = DateTime.UtcNow;
            ReplacedByTokenHash = replacedByTokenHash;
        }
    }
}
