using AuthSketch.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthSketch.Persistence.Configurations;

public sealed class RefreshTokenConfig : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");
        builder.HasKey(x => x.Id);

        builder.Ignore(x => x.IsExpired);
        builder.Ignore(x => x.IsRevoked);

        builder
            .Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .UseIdentityColumn()
            .HasIdentityOptions(startValue: 1, incrementBy: 1);

        builder
            .Property(x => x.CreatedAt)
            .IsRequired();

        builder
            .Property(x => x.CreatedByIp)
            .HasMaxLength(50);

        builder
            .Property(x => x.Token)
            .IsRequired();

        builder
            .Property(x => x.ExpiresAt)
            .IsRequired();

        builder
            .Property(x => x.RevokedByIp)
            .HasMaxLength(50);

        builder
            .Property(x => x.UserId)
            .IsRequired();
    }
}
