using Domain;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Authentication> Authentications => Set<Authentication>();
    public DbSet<AccessRight> AccessRights => Set<AccessRight>();
    public DbSet<Application> Applications => Set<Application>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IDataAssemblyMarker).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
