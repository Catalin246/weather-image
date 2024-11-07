using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace WeatherImage.Functions.JobStart.GetImages
{
    public class GetImages
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly ILogger<GetImages> _logger;

        public GetImages(BlobServiceClient blobServiceClient, ILogger<GetImages> logger)
        {
            _blobServiceClient = blobServiceClient;
            _logger = logger;
        }

        [Function("GetImages")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "images")] HttpRequest req)
        {
            _logger.LogInformation("Fetching list of weather images.");

            // Specify the blob container name
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient("weather-image-public");

            // List all blobs in the specified container
            var imageUrls = new List<string>();
            await foreach (BlobItem blobItem in blobContainerClient.GetBlobsAsync())
            {
                // Create a BlobClient to retrieve the URL of each blob
                var blobClient = blobContainerClient.GetBlobClient(blobItem.Name);
                imageUrls.Add(blobClient.Uri.ToString());
            }

            // Return the list of image URLs as JSON
            return new OkObjectResult(new { images = imageUrls });
        }
    }
}
