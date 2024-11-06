using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
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
    public class ProcessImage
    {
        private readonly ILogger<ProcessImage> _logger;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly UnsplashImageService _unsplashImageService;
        private readonly TableClient _tableClient;

        public ProcessImage(
            ILogger<ProcessImage> logger,
            BlobServiceClient blobServiceClient,
            UnsplashImageService unsplashImageService,
            TableClient tableClient)

        {
            _logger = logger;
            _blobServiceClient = blobServiceClient;
            _unsplashImageService = unsplashImageService;
            _tableClient = tableClient;
        }

        [Function("ProcessImage")]
        public async Task RunAsync(
            [QueueTrigger("process-image", Connection = "AzureWebJobsStorage")] QueueMessage imageMessage)
        {
            _logger.LogInformation("Processing image from image-processing queue.");

            var jobData = JsonSerializer.Deserialize<JobData>(imageMessage.MessageText);
            if (jobData == null)
            {
                _logger.LogWarning("Image process message deserialized to null.");
                return;
            }

            // Update Table Storage with job status as "In Progress"
            await UpdateJobStatusAsync(jobData.JobId, "In Progress");

            // Generate and upload the weather image
            var blobUrl = await GenerateWeatherImageAsync(jobData.Station, jobData.JobId);

            // Update Table Storage with job status as "Completed"
            await UpdateJobStatusAsync(jobData.JobId, "Completed");

            _logger.LogInformation($"Image processing completed. Blob URL: {blobUrl}");
        }

        private async Task<string> GenerateWeatherImageAsync(StationMeasurement stationData, string jobId)
        {
            _logger.LogInformation($"Generating weather image for station: {stationData.StationName}");

            // Fetch background image
            var imageUrl = await _unsplashImageService.GetRandomImageUrlAsync("netherlands");
            using var imageStream = await new HttpClient().GetStreamAsync(imageUrl);

            // Text overlays
            var textOverlays = new[]
            {
                ($"Station: {stationData.StationName}", (10f, 40f), 20, "#000000"),
                ($"Temperature: {stationData.Temperature}°C", (10f, 80f), 20, "#000000")
            };

            // Add text to image
            using var finalImageStream = ImageEditor.AddTextToImage(imageStream, textOverlays);

            // Upload to Blob Storage with jobId in metadata
            string blobName = $"{stationData.StationName.Replace(" ", "_")}_{Guid.NewGuid()}.png";
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient("weather-image-public");
            await blobContainerClient.CreateIfNotExistsAsync();

            var blobClient = blobContainerClient.GetBlobClient(blobName);
            var metadata = new Dictionary<string, string> { { "jobId", jobId } };

            await blobClient.UploadAsync(finalImageStream, overwrite: true);
            await blobClient.SetMetadataAsync(metadata);

            _logger.LogInformation($"Image uploaded with jobId metadata: {blobName}");
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
