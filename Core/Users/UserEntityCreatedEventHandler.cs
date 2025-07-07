using Data;
using Domain.AuditLogs;
using Domain.Events.Core;
using Domain.Users;
using Infrastructure.NotificationPublisherService;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Users;

public sealed class UserEntityCreatedEventHandler : INotificationHandler<DomainEvent>
{
    private readonly ApplicationDbContext _dbContext;

    public UserEntityCreatedEventHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(DomainEvent notification, CancellationToken cancellationToken)
    {
        if (notification is not UserEntityCreatedEvent) return;

        var description = notification.ToString();
        if (!string.IsNullOrEmpty(description))
        {
            var log = new AuditLog(description);

            await _dbContext.AuditLogs.AddAsync(log, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
