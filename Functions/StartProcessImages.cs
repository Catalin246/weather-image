using System;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WeatherImage.Services;
using WeatherImage.Models;

namespace WeatherImage.Functions.StartProcessImages
{
     public class StartProcessImages
    {
        private readonly WeatherDataService _weatherDataService;
        private readonly QueueJobStartService _queueJobStartService;
        private readonly ILogger _logger;

        public StartProcessImages(WeatherDataService weatherDataService, QueueJobStartService queueJobStartService, ILoggerFactory loggerFactory)
        {
            _weatherDataService = weatherDataService;
            _queueJobStartService = queueJobStartService;
            _logger = loggerFactory.CreateLogger<StartProcessImages>();
        }

        [Function("StartProcessImages")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "start")] HttpRequestData req)
        {
            _logger.LogInformation("Starting weather image job.");

            // Get weather data from WeatherDataService
            var weatherData = await _weatherDataService.GetWeatherDataAsync();
        
            if (weatherData == null)
            {
                return req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
            }

            // Generate a unique job ID
            var jobId = Guid.NewGuid().ToString();
            weatherData.JobId = jobId;

            await _queueJobStartService.AddToQueueAsync(weatherData);

             _logger.LogInformation("Retrieving weather data successfully!");

            // Return a success response with job ID
            var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
            await response.WriteAsJsonAsync(new { jobId, statusUrl = $"/api/status/{jobId}" });
            return response;
        }
    }
}
