using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.NotificationPublisherService;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNotificationPublisher(this IServiceCollection services)
    {
        services.AddTransient<INotificationPublisher, NotificationPublisher>();

        return services;
    }
}
