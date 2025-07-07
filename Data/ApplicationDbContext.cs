using Domain.AccessRights;
using Domain.Applications;
using Domain.AuditLogs;
using Domain.Authentications;
using Domain.Users;
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
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IDataAssemblyMarker).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
