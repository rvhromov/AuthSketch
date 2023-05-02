using AuthSketch.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthSketch.Persistence.Configurations;

public sealed class ExternalAuthConfig : IEntityTypeConfiguration<ExternalAuth>
{
    public void Configure(EntityTypeBuilder<ExternalAuth> builder)
    {
        builder.ToTable("ExternalAuthProviders");
        builder.HasKey(x => x.Id);

        builder
            .Property(x => x.Provider)
            .IsRequired();

        builder
            .Property(x => x.AccessToken)
            .IsRequired();

        builder
            .Property(x => x.CreatedAt)
            .IsRequired();

        builder
            .Property(x => x.UserId)
            .IsRequired();
    }
}
