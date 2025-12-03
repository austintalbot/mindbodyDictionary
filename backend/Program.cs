

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();


builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

// Register CosmosClient for DI
builder.Services.AddSingleton(sp =>
{
    var connectionString = Environment.GetEnvironmentVariable("CosmosDbConnectionString");
    return new Microsoft.Azure.Cosmos.CosmosClient(connectionString);
});

await builder.Build().RunAsync();
