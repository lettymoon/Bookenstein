using Bookenstein.Domain.Entities;

namespace Bookenstein.Application.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken ct);
        Task<IEnumerable<RefreshToken>> GetActiveByUserAsync(Guid userId, CancellationToken ct);
        Task AddAsync(RefreshToken token, CancellationToken ct);
        Task<bool> SaveChangesAsync(CancellationToken ct);
    }
}
