// using System;
// using Newtonsoft.Json;
// using System.Collections.Generic;
// using System.Net;
// using System.Threading.Tasks;
// using Azure.Storage.Blobs;
// using Microsoft.Azure.Functions.Worker;
// using Microsoft.Azure.Functions.Worker.Http;
// using Microsoft.Extensions.Logging;
// using WeatherImage.Services;

// namespace WeatherImage.Functions.Status
// {
//     public class GetJobStatus
//     {
//         private readonly BlobUploaderService _blobUploaderService;
//         private readonly ILogger _logger;

//         public GetJobStatus(BlobUploaderService blobUploaderService, ILoggerFactory loggerFactory)
//         {
//             _blobUploaderService = blobUploaderService;
//             _logger = loggerFactory.CreateLogger<GetJobStatus>();
//         }

//         [Function("GetJobStatus")]
//         public async Task<HttpResponseData> Run(
//             [HttpTrigger(AuthorizationLevel.Function, "get", Route = "job/status/{jobId}")] HttpRequestData req,
//             string jobId)
//         {
//             _logger.LogInformation($"Checking job status for JobId: {jobId}");

//             List<string> imageUrls = await _blobUploaderService.GetImageUrlsAsync(jobId);

//             var response = req.CreateResponse(imageUrls.Count > 0 ? HttpStatusCode.OK : HttpStatusCode.Accepted);
//             // TO DO
//             //await response.WriteAsJsonAsync(imageUrls.Count > 0 ? imageUrls : "Job is still in progress.");
//             await response.WriteAsJsonAsync("Job is still in progress.");
//             return response;
//         }
//     }
// }

