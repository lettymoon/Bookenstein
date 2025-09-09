namespace Bookenstein.Domain.Entities;

public class User : Entity
{
    public string Name { get; private set; } = null!;
    public string Username { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string Role { get; private set; } = "User";
    public string? City { get; private set; }
    public string? Bio { get; private set; }
    public string? AvatarUrl { get; private set; }
    public string PasswordHash { get; set; } = null!;
    public string SecurityStamp { get; set; } = Guid.NewGuid().ToString("N");

    public DateTime PasswordUpdatedAt { get; set; } = DateTime.UtcNow;

    private User() { }
    public User(string name, string username, string email, string role, string passwordHash){
        SetName(name);
        SetUsername(username);
        SetEmail(email);
        SetRole(role);
        ChangePassword(passwordHash);
    }

    public void UpdateProfile(string name) { SetName(name); UpdatedAt = DateTime.UtcNow; }
    public void ChangePassword(string newHash)
    {
        if (string.IsNullOrWhiteSpace(newHash)) throw new ArgumentException("Hash inválido");
        PasswordHash = newHash;
        PasswordUpdatedAt = DateTime.UtcNow;
        SecurityStamp = Guid.NewGuid().ToString("N");
        UpdatedAt = DateTime.UtcNow;
    }

    private void SetName(string name) => Name = name.Trim();
    private void SetUsername(string u) => Username = u.Trim().ToLowerInvariant();
    private void SetEmail(string e) => Email = e.Trim().ToLowerInvariant();
    private void SetRole(string role) => Role = role.Trim();

}
