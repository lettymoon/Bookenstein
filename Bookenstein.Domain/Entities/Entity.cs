namespace Bookenstein.Domain.Entities
{
    public class Entity
    {
        public Guid Id { get; protected set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
        public DateTimeOffset? UpdatedAt { get; protected set; } = DateTime.UtcNow;
    }
}
