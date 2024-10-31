using System.Threading.Tasks;
using WeatherImage.Models;

namespace WeatherImage.Services
{
    public class WeatherDataService
    {
        public async Task<WeatherData> GetWeatherDataAsync()
        {
            // Simulate data retrieval
            return await Task.FromResult(new WeatherData
            {
                Actual = new ActualData
                {
                    StationMeasurements = new System.Collections.Generic.List<StationMeasurement>
                    {
                        new StationMeasurement { Name = "Station1", Temperature = 20.5 },
                        new StationMeasurement { Name = "Station2", Temperature = 18.3 }
                    }
                }
            });
        }
    }
}
