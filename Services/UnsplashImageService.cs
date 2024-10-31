using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using WeatherImage.Models;

namespace WeatherImage.Services
{
    public class UnsplashImageService
    {
        private readonly HttpClient _httpClient;
        private readonly string? _unsplashApiKey;

        public UnsplashImageService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _unsplashApiKey = configuration["UnsplashApiKey"];
        }

        public async Task<string?> GetRandomImageUrlAsync(string query = "sky")
        {
            var requestUrl = $"https://api.unsplash.com/photos/random?query={query}&client_id={_unsplashApiKey}";

            var response = await _httpClient.GetFromJsonAsync<UnsplashResponse>(requestUrl);
            return response?.Urls?.Regular;
        }
    }

    public class UnsplashResponse
    {
        public required UnsplashUrls Urls { get; set; }
    }

    public class UnsplashUrls
    {
        public required string Regular { get; set; }
    }
}
