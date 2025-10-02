using Microsoft.Extensions.DependencyInjection;
using NotificationService.Domain.Interfaces;

namespace NotificationService.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register application services
        services.AddScoped<INotificationService, Services.NotificationService>();

        return services;
    }
}
