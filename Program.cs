using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WeatherImage.Services;
using Azure.Data.Tables;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration(config =>
    {
        // Load local.settings.json for local development
        config.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
              .AddEnvironmentVariables();
    })
    .ConfigureServices(services => {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddHttpClient();
        services.AddSingleton<WeatherDataService>();
        services.AddSingleton<QueueJobStartService>();
        services.AddSingleton<QueueProcessImageService>();
        services.AddSingleton<UnsplashImageService>();
        services.AddSingleton(new BlobServiceClient(Environment.GetEnvironmentVariable("AzureWebJobsStorage")));

        services.AddSingleton(x =>
        {
            string? connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("AzureWebJobsStorage connection string is not set in environment variables.");
            }
            var tableServiceClient = new TableServiceClient(connectionString);
            var tableClient = tableServiceClient.GetTableClient("JobStatusTable");
            tableClient.CreateIfNotExists();
            return tableClient;
        });

    })
    .Build();

host.Run();