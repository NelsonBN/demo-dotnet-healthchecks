using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Demo.Api;

public sealed class StartupBackgroundService(
    ILogger<StartupBackgroundService> logger,
    StartupHealthCheck healthCheck,
    IMongoDatabase database) : BackgroundService
{
    private readonly ILogger<StartupBackgroundService> _logger = logger;
    private readonly StartupHealthCheck _healthCheck = healthCheck;
    private readonly IMongoDatabase _database = database;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[API] Configuring...");

        // To simulate a long startup process
        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

        _database.CreateCollection(
            nameof(Product),
            cancellationToken: stoppingToken);

        _healthCheck.StartupCompleted = true;

        _logger.LogInformation("[API] Configured");
    }
}
