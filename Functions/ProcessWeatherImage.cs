using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using WeatherImage.Models;
using WeatherImage.Services;
using WeatherImage.Utilities.ImageEditor;

namespace WeatherImage.Functions.ProcessImage
{
    public class ProcessWeatherImage
    {
        private readonly ILogger<ProcessWeatherImage> _logger;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly UnsplashImageService _unsplashImageService;
        private readonly TableClient _tableClient;

        public ProcessWeatherImage(
            ILogger<ProcessWeatherImage> logger,
            BlobServiceClient blobServiceClient,
            UnsplashImageService unsplashImageService,
            TableClient tableClient)
        {
            _logger = logger;
            _blobServiceClient = blobServiceClient;
            _unsplashImageService = unsplashImageService;
            _tableClient = tableClient;
        }

        [Function(nameof(ProcessWeatherImage))]
        public async Task RunAsync([QueueTrigger("weather-image-queue", Connection = "AzureWebJobsStorage")] QueueMessage message)
        {
            try
            {
                _logger.LogInformation("Processing message from weather-image-queue.");

                // Deserialize the message content
                var jobData = JsonSerializer.Deserialize<JobData>(message.MessageText);
                if (jobData == null)
                {
                    _logger.LogWarning("Deserialized message is null.");
                    return;
                }

                // Update Table Storage with job status as "In Progress"
                await UpdateJobStatusAsync(jobData.JobId, "In Progress");

                // Generate and upload the weather image
                await GenerateWeatherImageAsync(jobData.Station);

                // Update Table Storage with job status as "Completed"
                await UpdateJobStatusAsync(jobData.JobId, "Completed");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing queue message: {ex.Message}");
            }
        }

        private async Task<string> GenerateWeatherImageAsync(StationMeasurement stationData)
        {
            _logger.LogInformation($"Generating weather image for station: {stationData.StationName}");

            // Fetch an Unsplash image to use as the background
            var imageUrl = await _unsplashImageService.GetRandomImageUrlAsync("netherlands");
            using var imageStream = await new HttpClient().GetStreamAsync(imageUrl);

            // Prepare text overlays
            var textOverlays = new[]
            {
                ($"Station: {stationData.StationName}", (10f, 40f), 20, "#000000"),
                ($"Temperature: {stationData.Temperature}°C", (10f, 80f), 20, "#000000")
            };

            // Use ImageHelper to add text to the background image
            using var finalImageStream = ImageEditor.AddTextToImage(imageStream, textOverlays);

            // Upload the final image to Azure Blob Storage
            string blobContainerName = "weather-image-public";
            string blobName = $"{stationData.StationName.Replace(" ", "_")}_{Guid.NewGuid()}.png";

            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(blobContainerName);
            await blobContainerClient.CreateIfNotExistsAsync();

            var blobClient = blobContainerClient.GetBlobClient(blobName);
            await blobClient.UploadAsync(finalImageStream, overwrite: true);

            _logger.LogInformation($"Image uploaded successfully as {blobName} in container {blobContainerName}.");
            return blobClient.Uri.ToString(); 
        }

        private async Task UpdateJobStatusAsync(string jobId, string status)
        {
            var jobStatusEntity = new JobStatusEntity(jobId)
            {
                Status = status,
            };

            await _tableClient.UpsertEntityAsync(jobStatusEntity);
            _logger.LogInformation($"Job status for Job ID {jobId} updated to '{status}'.");
        }
    }
}
