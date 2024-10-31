// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Threading.Tasks;
// using Azure.Storage.Blobs;
// using Azure.Storage.Blobs.Models;
// using Microsoft.Extensions.Configuration;

// namespace WeatherImage.Services
// {
//     public class BlobUploaderService
//     {
//         private readonly BlobContainerClient _containerClient;

//         public BlobUploaderService(IConfiguration configuration)
//         {
//             string connectionString = configuration["AzureWebJobsStorage"];
//             _containerClient = new BlobContainerClient(connectionString, "weather-images");
//             _containerClient.CreateIfNotExists(PublicAccessType.Blob);
//         }

//         public async Task UploadImageAsync(Stream imageStream, string blobName)
//         {
//             BlobClient blobClient = _containerClient.GetBlobClient(blobName);
//             await blobClient.UploadAsync(imageStream, overwrite: true);
//         }

//         public async Task<List<string>> GetImageUrlsAsync(string jobId)
//         {
//             List<string> imageUrls = new List<string>();

//             await foreach (BlobItem blobItem in _containerClient.GetBlobsAsync(prefix: $"{jobId}/"))
//             {
//                 BlobClient blobClient = _containerClient.GetBlobClient(blobItem.Name);
//                 imageUrls.Add(blobClient.Uri.ToString());
//             }

//             return imageUrls;
//         }
//     }
// }
