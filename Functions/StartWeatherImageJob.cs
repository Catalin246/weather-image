using System;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WeatherImage.Services;
using WeatherImage.Models;

namespace WeatherImage.Functions.StartWeatherImageJob
{
     public class StartWeatherImageJob
    {
        private readonly WeatherDataService _weatherDataService;
        private readonly QueueWriterService _queueWriterService;
        private readonly ILogger _logger;

        public StartWeatherImageJob(WeatherDataService weatherDataService, QueueWriterService queueWriterService, ILoggerFactory loggerFactory)
        {
            _weatherDataService = weatherDataService;
            _queueWriterService = queueWriterService;
            _logger = loggerFactory.CreateLogger<StartWeatherImageJob>();
        }

        [Function("StartWeatherImageJob")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "start-weather-image-job")] HttpRequestData req)
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

            // Enqueue each station's data
            foreach (var station in weatherData.Actual.StationMeasurements)
            {
                var jobData = new JobData { JobId = jobId, Station = station };
                await _queueWriterService.AddToQueueAsync(jobData);
            }

             _logger.LogInformation("Retrieving weather data successfully!");

            // Return a success response with job ID
            var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
            await response.WriteAsJsonAsync(new { jobId, statusUrl = $"/api/job/status/{jobId}" });
            return response;
        }
    }
}
