namespace WeatherImage.Models
{
    public class JobData
    {
        public required string JobId { get; set; }
        public required StationMeasurement Station { get; set; }
    }
}
