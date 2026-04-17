using ExperimentPlatform.Extensions;
using ExperimentPlatformApplication.Abstractions;
using ExperimentPlatformInfrastructure;
using ExperimentPlatformInfrastructure.Background;
using ExperimentPlatformInfrastructure.Persistence;
using ExperimentPlatformWorker;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddExperimentPlatformBackgroundWorker();

// Configure queue settings
builder.Services.Configure<QueueSettings>(
    builder.Configuration.GetSection(QueueSettings.SectionName));

builder.Services.Configure<RabbitMqSettings>(
    builder.Configuration.GetSection(RabbitMqSettings.SectionName));

var queueSettings = builder.Configuration
    .GetSection(QueueSettings.SectionName)
    .Get<QueueSettings>() ?? new QueueSettings();

var queueType = queueSettings.GetQueueType();

// Register only the selected queue implementation
if (queueType == QueueType.RabbitMQ)
{
    builder.Services.AddSingleton<IBackgroundQueue, RabbitMqBackgroundQueue>();
}
else
{
    builder.Services.AddSingleton<IBackgroundQueue, InMemoryBackgroundQueue>();
}

builder.Services.AddHealthChecks();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ExperimentDbContext>();

    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddlewareDefaults();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/health", () => "OK");

app.Run();
