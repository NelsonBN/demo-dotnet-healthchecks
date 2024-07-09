using Demo.Api;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;


var builder = WebApplication.CreateSlimBuilder(args);


builder.Services
    .AddHealthChecks()
    .AddMongoDb(
        sp => sp.GetRequiredService<IMongoClient>(),
        "MongoDB",
        HealthStatus.Unhealthy)
    .AddCheck<StartupHealthCheck>(
        "Startup",
        tags: ["Startup"]);


builder.Services
    .AddSingleton(sp => new MongoUrl(sp.GetRequiredService<IConfiguration>().GetConnectionString("Default")!))
    .AddSingleton<IMongoClient>(sp =>
    {
        var url = sp.GetRequiredService<MongoUrl>();
        return new MongoClient(url);
    })
    .AddSingleton(sp =>
    {
        var url = sp.GetRequiredService<MongoUrl>();
        var client = sp.GetRequiredService<IMongoClient>();
        return client.GetDatabase(url.DatabaseName);
    })
    .AddSingleton(sp => sp
        .GetRequiredService<IMongoDatabase>()
        .GetCollection<Product>(nameof(Product)));


builder.Services
    .AddSingleton<StartupHealthCheck>()
    .AddHostedService<StartupBackgroundService>();


var app = builder.Build();


app.MapHealthChecks("/healthz/startup", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("Startup"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/healthz/live", new HealthCheckOptions
{
    Predicate = _ => false,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/healthz/ready", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapProductsEndpoints();

await app.RunAsync();
