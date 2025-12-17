using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services => {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
    })
    // Register CosmosClient for DI
    .ConfigureServices(services => {
        services.AddSingleton((s) => {
            var configuration = s.GetRequiredService<Microsoft.Extensions.Configuration.IConfiguration>();
            var connectionString = configuration["CONNECTION_COSMOSDB"];
            return new Microsoft.Azure.Cosmos.CosmosClient(connectionString);
        });
    })    
    .Build();

host.Run();
