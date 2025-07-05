using Domain.Authentications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public class AuthenticationConfiguration : IEntityTypeConfiguration<Authentication>
{
    public void Configure(EntityTypeBuilder<Authentication> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Username)
               .IsRequired()
               .HasMaxLength(100);

        builder.HasIndex(a => a.Username)
               .IsUnique();
    }
}
