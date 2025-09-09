using Bookenstein.Application.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookenstein.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> SignupAsync(SignupRequest req, CancellationToken ct);
        Task<AuthResponse> LoginAsync(LoginRequest req, CancellationToken ct);
        Task<AuthResponse> RefreshAsync(RefreshRequest req, CancellationToken ct);
        Task LogoutAsync(RefreshRequest req, CancellationToken ct);
        Task ChangePasswordAsync(Guid userId, ChangePasswordRequest req, CancellationToken ct);
    }
}
