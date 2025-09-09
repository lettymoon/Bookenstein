using Bookenstein.Application.Interfaces;
using Bookenstein.Domain.Entities;
using Bookenstein.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Bookenstein.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _ctx;
        public UserRepository(AppDbContext ctx) => _ctx = ctx;

        public Task<User?> GetByIdAsync(Guid id, CancellationToken ct)
            => _ctx.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct);

        public Task<User?> GetByEmailAsync(string email, CancellationToken ct)
            => _ctx.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email, ct);
        public Task<User?> GetByUsernameAsync(string username, CancellationToken ct)
            => _ctx.Users.FirstOrDefaultAsync(u => u.Username == username, ct);
        public Task<List<User>> GetAllAsync(CancellationToken ct)
            => _ctx.Users.AsNoTracking().ToListAsync(ct);

        public Task AddAsync(User user, CancellationToken ct)
            => _ctx.Users.AddAsync(user, ct).AsTask();

        public async Task<bool> SaveChangesAsync(CancellationToken ct)
            => await _ctx.SaveChangesAsync(ct) > 0;
    }
}
