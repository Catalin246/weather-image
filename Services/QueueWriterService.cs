using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using WeatherImage.Models;

namespace WeatherImage.Services
{
    public class QueueWriterService
    {
        private readonly QueueClient _queueClient;

        public QueueWriterService(IConfiguration configuration)
        {
            var connectionString = configuration["AzureWebJobsStorage"];
            _queueClient = new QueueClient(connectionString, "weather-image-queue");

            // Ensure the queue exists
            _queueClient.CreateIfNotExists();
        }

        public async Task AddToQueueAsync(JobData jobData)
        {
            if (jobData == null)
                throw new ArgumentNullException(nameof(jobData));

            var message = JsonConvert.SerializeObject(jobData);
            var bytes = Encoding.UTF8.GetBytes(message);
            await _queueClient.SendMessageAsync(Convert.ToBase64String(bytes));
        }
    }
}
