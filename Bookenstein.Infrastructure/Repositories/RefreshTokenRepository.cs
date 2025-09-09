using Bookenstein.Application.Interfaces;
using Bookenstein.Domain.Entities;
using Bookenstein.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookenstein.Infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _ctx;
        public RefreshTokenRepository(AppDbContext ctx) => _ctx = ctx;

        public Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken ct)
            => _ctx.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == tokenHash, ct);

        public async Task<IEnumerable<RefreshToken>> GetActiveByUserAsync(Guid userId, CancellationToken ct)
            => await _ctx.RefreshTokens.Where(t => t.UserId == userId && t.RevokedAt == null && t.ExpiresAt > DateTime.UtcNow).ToListAsync(ct);

        public Task AddAsync(RefreshToken token, CancellationToken ct)
            => _ctx.RefreshTokens.AddAsync(token, ct).AsTask();

        public async Task<bool> SaveChangesAsync(CancellationToken ct)
            => await _ctx.SaveChangesAsync(ct) > 0;
    }
}
