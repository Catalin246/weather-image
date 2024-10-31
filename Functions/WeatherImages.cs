using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace WeatherImage.Functions.GetImages
{
    public class WeatherImages
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly ILogger<WeatherImages> _logger;

        public WeatherImages(BlobServiceClient blobServiceClient, ILogger<WeatherImages> logger)
        {
            _blobServiceClient = blobServiceClient;
            _logger = logger;
        }

        [Function("WeatherImages")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "images")] HttpRequestData req)
        {
            _logger.LogInformation("Fetching list of weather images.");

            // Retrieve blob container
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient("weather-image-public");

            // List blobs in container
            var imageUrls = new List<string>();
            await foreach (BlobItem blobItem in blobContainerClient.GetBlobsAsync())
            {
                var blobClient = blobContainerClient.GetBlobClient(blobItem.Name);
                imageUrls.Add(blobClient.Uri.ToString());
            }

            // Return list of image URLs as JSON response
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            await response.WriteStringAsync(JsonSerializer.Serialize(new { images = imageUrls }));

            return response;
        }
    }
}
