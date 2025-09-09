using Bookenstein.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookenstein.Infrastructure.Mappings;
public class RefreshTokenMap : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> e)
    {
        e.ToTable("RefreshTokens");
        e.HasKey(x => x.Id);
        e.Property(x => x.TokenHash).IsRequired();
        e.Property(x => x.ExpiresAt).IsRequired();
        e.HasIndex(x => x.TokenHash).IsUnique();
        e.HasIndex(x => x.UserId);
    }
}
