using Microsoft.Extensions.DependencyInjection;

namespace ExperimentPlatformWorker;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Runs dequeue/processing for the shared <c>IBackgroundQueue</c> in the same process as the host (e.g. the API),
    /// so an in-memory queue implementation is shared correctly.
    /// </summary>
    public static IServiceCollection AddExperimentPlatformBackgroundWorker(this IServiceCollection services)
    {
        services.AddHostedService<BackgroundQueueWorkerHostedService>();
        return services;
    }
}
