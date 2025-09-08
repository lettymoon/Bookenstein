namespace Bookenstein.Domain.Entities;

public class Users : Entity
{
    public string Name { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? City { get; set; }
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public string Role { get; set; } = "user";
    public DateTimeOffset? UpdatedAt { get; set; }

}
