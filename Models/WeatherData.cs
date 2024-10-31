using System.Collections.Generic;

namespace WeatherImage.Models
{
    public class WeatherData
    {
        public required ActualData Actual { get; set; }
    }

    public class ActualData
    {
        public required List<StationMeasurement> StationMeasurements { get; set; }
    }

    public class StationMeasurement
    {
        public required string Name { get; set; }
        public double Temperature { get; set; }
    }
}
