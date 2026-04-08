using ExperimentPlatform.Extensions;
using ExperimentPlatformInfrastructure;
using ExperimentPlatformInfrastructure.Persistence;
using ExperimentPlatformWorker;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddHealthChecks();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddExperimentPlatformBackgroundWorker();

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
