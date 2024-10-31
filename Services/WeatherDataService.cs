using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WeatherImage.Models;

namespace WeatherImage.Services
{
    public class WeatherDataService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<WeatherDataService> _logger;

        public WeatherDataService(HttpClient httpClient, ILogger<WeatherDataService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<WeatherData> GetWeatherDataAsync()
        {
            try
            {
                // Fetch data from Buienradar API
                var apiData = await _httpClient.GetFromJsonAsync<WeatherData>("https://data.buienradar.nl/2.0/feed/json");

                if (apiData?.Actual?.StationMeasurements == null)
                {
                    _logger.LogWarning("No station measurements found in the response.");
                    return new WeatherData { Actual = new ActualData { StationMeasurements = new List<StationMeasurement>() } };
                }

                // Map to simplified WeatherData format
                var weatherData = new WeatherData
                {
                    Actual = new ActualData
                    {
                        StationMeasurements = apiData.Actual.StationMeasurements
                            .Select(station => new StationMeasurement
                            {
                                StationName = station.StationName,
                                Temperature = station.Temperature ?? 0
                            })
                            .ToList()
                    }
                };

                _logger.LogInformation("Successfully mapped weather data.");
                return weatherData;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving or mapping weather data: {ex.Message}");
                return new WeatherData { Actual = new ActualData { StationMeasurements = new List<StationMeasurement>() } };
            }
        }
    }
}
