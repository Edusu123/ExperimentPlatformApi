using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace ExperimentPlatformInfrastructure.Persistence
{
    public class ExperimentDbContextFactory : IDesignTimeDbContextFactory<ExperimentDbContext>
    {
        public ExperimentDbContext CreateDbContext(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile(
                    $"appsettings.{environment}.json",
                    optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString("db");

            var optionsBuilder =
                new DbContextOptionsBuilder<ExperimentDbContext>();

            optionsBuilder.UseNpgsql(connectionString);

            return new ExperimentDbContext(optionsBuilder.Options);
        }
    }
}
