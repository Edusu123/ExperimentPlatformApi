using ExperimentPlatformApplication.Abstractions;
using ExperimentPlatformDomain.Interfaces;
using ExperimentPlatformInfrastructure.Background;
using ExperimentPlatformInfrastructure.Persistence;
using ExperimentPlatformInfrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExperimentPlatformInfrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<ExperimentDbContext>(o =>
                o.UseNpgsql(config.GetConnectionString("db")));

            services.AddScoped<IExperimentRepository, ExperimentRepository>();
            services.AddScoped<IEventRepository, EventRepository>();

            services.AddSingleton<IBackgroundQueue, InMemoryBackgroundQueue>();

            return services;
        }
    }
}
