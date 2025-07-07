using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.NotificationPublisherService;

public interface INotificationHandler<in TNotification>
{
    Task Handle(TNotification notification, CancellationToken cancellationToken);
}

public interface INotificationPublisher
{
    Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken);
}

// Use a custom notification publisher since MediatR requires that domain events implement INotification.
// This one does not.
public class NotificationPublisher : INotificationPublisher
{
    private readonly IServiceProvider _serviceProvider;

    public NotificationPublisher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);

        var handlers = _serviceProvider.GetServices<INotificationHandler<TNotification>>();

        foreach (var handler in handlers)
        {
            await handler.Handle(notification, cancellationToken);
        }
    }
}
