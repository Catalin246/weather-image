using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using WeatherImage.Models;

namespace WeatherImage.Functions.JobStatus
{
    public class JobStatus
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly ILogger<JobStatus> _logger;
        private readonly TableClient _tableClient;

        public JobStatus(BlobServiceClient blobServiceClient, TableClient tableClient, ILogger<JobStatus> logger)
        {
            _blobServiceClient = blobServiceClient;
            _tableClient = tableClient;
            _logger = logger;
        }

        [Function("JobStatus")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "status/{jobId}")] HttpRequestData req, string jobId)
        {
            _logger.LogInformation("Fetching list of weather images for jobId: {jobId}", jobId);

            // Retrieve job status from Table Storage
            var jobStatusEntity = await _tableClient.GetEntityAsync<JobStatusEntity>("JobStatus", jobId);
            if (jobStatusEntity == null || jobStatusEntity.Value.Status != "Completed")
            {
                // If job is still in progress or status is not found, return "In Progress" status
                var inProgressResponse = req.CreateResponse(HttpStatusCode.OK);
                await inProgressResponse.WriteStringAsync(JsonSerializer.Serialize(new { status = "In Progress" }));
                return inProgressResponse;
            }

            // Retrieve blob container
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient("weather-image-public");

            // List blobs in container and filter by jobId
            var imageUrls = new List<string>();
            await foreach (BlobItem blobItem in blobContainerClient.GetBlobsAsync())
            {
                var blobClient = blobContainerClient.GetBlobClient(blobItem.Name);

                // Retrieve blob properties to access metadata
                var blobProperties = await blobClient.GetPropertiesAsync();

                // Check if the blob's metadata contains the correct jobId
                if (blobProperties.Value.Metadata.TryGetValue("jobId", out var blobJobId) && blobJobId == jobId)
                {
                    imageUrls.Add(blobClient.Uri.ToString());
                }
            }

            // Return list of image URLs as JSON response
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            await response.WriteStringAsync(JsonSerializer.Serialize(new { images = imageUrls }));

            return response;
        }
    }
}
