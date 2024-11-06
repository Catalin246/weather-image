using System;
using System.Text.Json;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using WeatherImage.Models;
using WeatherImage.Services;

namespace WeatherImage.Functions.JobStart
{
    public class JobStarter
{
    private readonly ILogger<JobStarter> _logger;
    private readonly QueueProcessImageService _queueProcessImageService; 

    public JobStarter(ILogger<JobStarter> logger, QueueProcessImageService queueProcessImageService)
    {
        _logger = logger;
        _queueProcessImageService = queueProcessImageService;
    }

    [Function("JobStarter")]
    public async Task RunAsync(
        [QueueTrigger("job-start", Connection = "AzureWebJobsStorage")] QueueMessage jobMessage)
    {
        _logger.LogInformation("Starting job processing for job ID.");

        var weatherData = JsonSerializer.Deserialize<WeatherData>(jobMessage.MessageText);
        if (weatherData == null)
        {
            _logger.LogWarning("Job message deserialized to null.");
            return;
        }
        
        // For each weather station, enqueue a message for image processing
        foreach (var station in weatherData.Actual.StationMeasurements)
        {
            var jobData = new JobData { JobId = weatherData.JobId, Station = station };
            await _queueProcessImageService.AddToQueueAsync(jobData);
        }

        _logger.LogInformation("Enqueued image processing messages");
    }
}

}
