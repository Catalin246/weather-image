using Azure;
using Azure.Data.Tables;
using System;

namespace WeatherImage.Models
{
    public class JobStatusEntity : ITableEntity
    {
        public JobStatusEntity() { }

        public JobStatusEntity(string jobId)
        {
            PartitionKey = "JobStatus"; 
            RowKey = jobId;             
        }

        public string? Status { get; set; }
        public string? PartitionKey { get; set; }
        public string? RowKey { get; set; }
        public ETag ETag { get; set; }
        DateTimeOffset? ITableEntity.Timestamp { get; set; }
    }
}
