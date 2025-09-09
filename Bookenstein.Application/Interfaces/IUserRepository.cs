using Bookenstein.Domain.Entities;

namespace Bookenstein.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct);
    Task<User?> GetByUsernameAsync(string username, CancellationToken ct);
    Task<List<User>> GetAllAsync(CancellationToken ct);
    Task AddAsync(User users, CancellationToken ct);
    Task<bool> SaveChangesAsync(CancellationToken ct);
}