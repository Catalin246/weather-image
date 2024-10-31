using System;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace WeatherImage.Functions.ProcessImage
{
    public class ProcessWeatherImage
    {
        private readonly ILogger<ProcessWeatherImage> _logger;

        public ProcessWeatherImage(ILogger<ProcessWeatherImage> logger)
        {
            _logger = logger;
        }

        [Function(nameof(ProcessWeatherImage))]
        public void Run([QueueTrigger("weather-image-queue", Connection = "AzureWebJobsStorage")] QueueMessage message)
        {
            _logger.LogInformation($"C# Queue trigger function processed: {message.MessageText}");
        }
    }
}
