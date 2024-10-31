using System.Collections.Generic;

namespace WeatherImage.Models
{
    // Simplified format classes
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

    // Classes for deserialization from Buienradar API
    public class BuienRadarApiResponse
    {
        public required ActualDataApi Actual { get; set; }
    }

    public class ActualDataApi
    {
        public required List<StationMeasurementApi> StationMeasurements { get; set; }
    }

    public class StationMeasurementApi
    {
        public required string StationName { get; set; }
        public double? Temperature { get; set; }
    }
}
