using Bookenstein.Domain.Entities;
using Bookenstein.Infrastructure.Mappings;
using Microsoft.EntityFrameworkCore;

namespace Bookenstein.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();


    protected override void OnModelCreating(ModelBuilder model)
    {
        base.OnModelCreating(model);
        model.ApplyConfiguration(new UserMap());
        model.ApplyConfiguration(new RefreshTokenMap());

        // USER
        model.Entity<User>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(120);
            e.Property(x => x.Username).IsRequired().HasMaxLength(40);
            e.Property(x => x.Email).IsRequired().HasMaxLength(160);
            e.HasIndex(x => x.Username).IsUnique();
            e.HasIndex(x => x.Email).IsUnique();

            e.Property(x => x.Role).IsRequired().HasMaxLength(20);
            e.Property(x => x.CreatedAt).HasDefaultValueSql("now() at time zone 'utc'");
        });


    }
}

