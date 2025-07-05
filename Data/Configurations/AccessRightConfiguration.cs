using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public class AccessRightConfiguration : IEntityTypeConfiguration<AccessRight>
{
    public void Configure(EntityTypeBuilder<AccessRight> builder)
    {
        builder.HasKey(ar => ar.Id);

        builder.Property(ar => ar.ApplicationUserId)
               .IsRequired();

        builder.HasOne(ar => ar.User)
               .WithMany(u => u.AccessRights)
               .HasForeignKey(ar => ar.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ar => ar.Application)
               .WithMany(a => a.AccessRights)
               .HasForeignKey(ar => ar.ApplicationId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
