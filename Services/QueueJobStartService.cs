using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using WeatherImage.Models;

namespace WeatherImage.Services
{
    public class QueueJobStartService
    {
        private readonly QueueClient _queueClient;

        public QueueJobStartService(IConfiguration configuration)
        {
            var connectionString = configuration["AzureWebJobsStorage"];
            _queueClient = new QueueClient(connectionString, "job-start");

            // Ensure the queue exists
            _queueClient.CreateIfNotExists();
        }

        public async Task AddToQueueAsync(WeatherData weatherData)
        {
            if (weatherData == null)
                throw new ArgumentNullException(nameof(weatherData));

            var message = JsonConvert.SerializeObject(weatherData);
            var bytes = Encoding.UTF8.GetBytes(message);
            await _queueClient.SendMessageAsync(Convert.ToBase64String(bytes));
        }
    }
}
