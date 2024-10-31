using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using WeatherImage.Models;
using WeatherImage.Utilities.ImageEditor;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace WeatherImage.Functions.ProcessImage
{
    public class ProcessWeatherImage
    {
        private readonly ILogger<ProcessWeatherImage> _logger;
        private readonly BlobServiceClient _blobServiceClient;

        public ProcessWeatherImage(ILogger<ProcessWeatherImage> logger, BlobServiceClient blobServiceClient)
        {
            _logger = logger;
            _blobServiceClient = blobServiceClient;
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

                // Log or process the weather data (for example purposes)
                // _logger.LogInformation($"Job ID: {jobData.JobId}");
                // _logger.LogInformation($"Station Name: {jobData.Station.Name}");
                // _logger.LogInformation($"Temperature: {jobData.Station.Temperature}°C");

                await GenerateWeatherImageAsync(jobData.Station);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing queue message: {ex.Message}");
            }
        }

        // Generate and upload a weather image
        private async Task GenerateWeatherImageAsync(StationMeasurement stationData)
        {
            _logger.LogInformation($"Generating weather image for station: {stationData.StationName}");

            // Create a blank image or load a base image file
            using var blankImage = new Image<Rgba32>(500, 300, new Rgba32(173, 216, 230)); // Light blue background
            using var imageStream = new MemoryStream();
            await blankImage.SaveAsPngAsync(imageStream);
            imageStream.Position = 0;

            // Prepare text overlays
            var textOverlays = new[]
            {
                ($"Station: {stationData.StationName}", (10f, 40f), 20, "#000000"), // Black text
                ($"Temperature: {stationData.Temperature}°C", (10f, 80f), 20, "#000000")
            };

            // Add text to the image using ImageHelper
            using var finalImageStream = ImageEditor.AddTextToImage(imageStream, textOverlays);

            // Upload the modified image to Azure Blob Storage
            string blobContainerName = "weather-images";
            string blobName = $"{stationData.StationName.Replace(" ", "_")}_{Guid.NewGuid()}.png";

            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(blobContainerName);
            await blobContainerClient.CreateIfNotExistsAsync();

            var blobClient = blobContainerClient.GetBlobClient(blobName);
            await blobClient.UploadAsync(finalImageStream, overwrite: true);

            _logger.LogInformation($"Image uploaded successfully as {blobName} in container {blobContainerName}.");
        }
    }
}
