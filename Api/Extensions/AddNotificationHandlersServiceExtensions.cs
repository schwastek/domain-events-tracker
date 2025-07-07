using Core.Users;
using Domain.Events.Core;
using Infrastructure.NotificationPublisherService;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Extensions;

public static class AddNotificationHandlersServiceExtensions
{
    public static IServiceCollection AddNotificationHandlers(this IServiceCollection services)
    {
        services.AddNotificationPublisher();
        services.AddTransient<INotificationHandler<DomainEvent>, UserEntityCreatedEventHandler>();

        return services;
    }
}
