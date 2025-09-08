using Bookenstein.Domain.Entities;
using System;

namespace Bookenstein.Application.Interfaces;

public interface IUserRepository
{
    Task<Users?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<Users?> GetByEmailAsync(string email, CancellationToken ct);
    Task AddAsync(Users users, CancellationToken ct);
    Task<bool> SaveChangesAsync(CancellationToken ct);
}