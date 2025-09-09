using System.Security.Cryptography;
using Bookenstein.Application.Contracts;
using Bookenstein.Domain.Entities;
using Bookenstein.Application.Interfaces;

namespace Bookenstein.Application.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IRefreshTokenRepository _refresh;
    private readonly IPasswordHasher _hasher;
    private readonly ITokenService _tokens;

    public AuthService(IUserRepository users, IRefreshTokenRepository refresh, IPasswordHasher hasher, ITokenService tokens)
        => (_users, _refresh, _hasher, _tokens) = (users, refresh, hasher, tokens);

    // Cadastro de usuário
    public async Task<AuthResponse> SignupAsync(SignupRequest request, CancellationToken ct)
    {
        // 1) Normaliza entrada
        var email = request.Email.Trim().ToLowerInvariant();
        var username = string.IsNullOrWhiteSpace(request.Username)
            ? email
            : request.Username!.Trim();

        // 2) Regras: unicidade
        if (await _users.GetByEmailAsync(email, ct) is not null)
            throw new InvalidOperationException("E-mail já cadastrado.");
        if (await _users.GetByUsernameAsync(username, ct) is not null)
            throw new InvalidOperationException("Nome de usuário já cadastrado.");

        // 3) Hash + entidade
        var hash = _hasher.Hash(request.Password);
        var user = new User(
            name: request.Name.Trim(),
            username: username,
            email: email,
            role: "Client",
            passwordHash: hash
        );

        // 4) Persiste user + refresh 
        await _users.AddAsync(user, ct);

        var (plainRefresh, refreshHash, refreshExp) = _tokens.CreateRefreshToken();
        await _refresh.AddAsync(new RefreshToken(user.Id, refreshHash, refreshExp), ct);

        await _users.SaveChangesAsync(ct);

        // 5) Gera access token
        var (access, exp, _) = _tokens.CreateAccessToken(user);
        return new AuthResponse(access, plainRefresh, exp);
    }

    // Login de usuário
    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        var user = await _users.GetByEmailAsync(request.EmailOrUsername, ct)
                   ?? await _users.GetByUsernameAsync(request.EmailOrUsername, ct)
                   ?? throw new UnauthorizedAccessException("Credenciais inválidas.");

        if (!_hasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Credenciais inválidas.");

        var (access, exp, _) = _tokens.CreateAccessToken(user);
        var (plain, rHash, rExp) = _tokens.CreateRefreshToken();
        await _refresh.AddAsync(new RefreshToken(user.Id, rHash, rExp), ct);
        await _refresh.SaveChangesAsync(ct);

        return new AuthResponse(access, plain, exp);
    }
    // Refresh do token de acesso

    public async Task<AuthResponse> RefreshAsync(RefreshRequest request, CancellationToken ct)
    {
        // calcula o hash do token recebido para comparar
        var tokenHash = Sha256(request.RefreshToken);
        var stored = await _refresh.GetByTokenHashAsync(tokenHash, ct)
                     ?? throw new UnauthorizedAccessException("Refresh token inválido.");

        if (!stored.IsActive)
            throw new UnauthorizedAccessException("Refresh token expirado ou revogado.");

        // rotação: revoga o atual e cria novo
        var (plainNew, hashNew, expNew) = _tokens.CreateRefreshToken();
        stored.Revoke(hashNew);
        await _refresh.AddAsync(new RefreshToken(stored.UserId, hashNew, expNew), ct);
        await _refresh.SaveChangesAsync(ct);

        // novo access token aqui
        var user = await _users.GetByIdAsync(stored.UserId, ct) ?? throw new UnauthorizedAccessException();
        var (access, expAccess, _) = _tokens.CreateAccessToken(user);

        return new AuthResponse(access, plainNew, expAccess);
    }
    // Logout (revoga o refresh token)

    public async Task LogoutAsync(RefreshRequest request, CancellationToken ct)
    {
        var tokenHash = Sha256(request.RefreshToken);
        var stored = await _refresh.GetByTokenHashAsync(tokenHash, ct);
        if (stored is null) return;
        stored.Revoke();
        await _refresh.SaveChangesAsync(ct);
    }
    // Função auxiliar para calcular o hash SHA-256

    private static string Sha256(string input)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(bytes);
    }

    // Redefinição de senha

    public async Task ChangePasswordAsync(Guid userId, ChangePasswordRequest req, CancellationToken ct)
    {
        var user = await _users.GetByIdAsync(userId, ct) ?? throw new UnauthorizedAccessException();

        // verifica senha atual
        if (!_hasher.Verify(req.CurrentPassword, user.PasswordHash))
            throw new UnauthorizedAccessException("Senha atual inválida.");

        // troca a senha
        var newHash = _hasher.Hash(req.NewPassword);
        user.ChangePassword(newHash);

        // revoga todos os refresh tokens ativos do usuário hehe
        var tokens = await _refresh.GetActiveByUserAsync(user.Id, ct);
        foreach (var t in tokens) t.Revoke();

        await _users.SaveChangesAsync(ct);  
        await _refresh.SaveChangesAsync(ct);
    }
}
