namespace Bookenstein.API.Models.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? City { get; set; }
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
        public string Role { get; set; } = "user";
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }

    }
}
