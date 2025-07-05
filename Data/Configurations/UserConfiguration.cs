using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.ObjectId)
               .IsRequired();

        builder.HasOne(u => u.Authentication)
               .WithOne()
               .HasForeignKey<User>(u => u.AuthenticationId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.AccessRights)
               .WithOne(ar => ar.User)
               .HasForeignKey(ar => ar.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}