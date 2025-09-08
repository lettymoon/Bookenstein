using Bookenstein.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookenstein.Infrastructure.Mappings
{
    public class UserMap : IEntityTypeConfiguration<Users>
    {
        public void Configure(EntityTypeBuilder<Users> e)
        {
            e.ToTable("users");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(120);
            e.Property(x => x.Username).IsRequired().HasMaxLength(40);
            e.Property(x => x.Email).IsRequired().HasMaxLength(160);
            e.Property(x => x.Role).IsRequired().HasMaxLength(20);
            e.HasIndex(x => x.Username).IsUnique();
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.CreatedAt).HasDefaultValueSql("now() at time zone 'utc'");
        }
    }
}
