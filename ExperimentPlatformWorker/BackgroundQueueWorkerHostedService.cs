using ExperimentPlatformApplication.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ExperimentPlatformWorker;

internal sealed class BackgroundQueueWorkerHostedService(IBackgroundQueue queue, IServiceProvider serviceProvider, ILogger<BackgroundQueueWorkerHostedService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Background queue worker started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var workItem = await queue.DequeueAsync(stoppingToken).ConfigureAwait(false);
                await using var scope = serviceProvider.CreateAsyncScope();
                await workItem(scope.ServiceProvider, stoppingToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error executing background queue work item");
            }
        }
    }
}
