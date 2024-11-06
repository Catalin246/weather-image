namespace WeatherImage.Models
{
    public class WeatherData
    {
        public required ActualData Actual { get; set; }

        public string? JobId { get; set; }
    }

    public class ActualData
    {
        public required List<StationMeasurement> StationMeasurements { get; set; }
    }

    public class StationMeasurement
    {
        public required string StationName { get; set; }
        public double? Temperature { get; set; }
    }
}
