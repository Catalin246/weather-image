// using System.Net.Http;
// using System.Net.Http.Json;
// using System.Threading.Tasks;
// using WeatherImage.Models;


// namespace WeatherImage.Services
// {
//     public class UnsplashImageService
//     {
//         private readonly HttpClient _httpClient;

//         public UnsplashImageService(HttpClient httpClient)
//         {
//             _httpClient = httpClient;
//         }

//         public async Task<string> GetRandomImageUrlAsync()
//         {
//             string unsplashApiKey = "<YOUR_UNSPLASH_API_KEY>";
//             string requestUri = $"https://api.unsplash.com/photos/random?query=weather&client_id={unsplashApiKey}";

//             var unsplashImage = await _httpClient.GetFromJsonAsync<UnsplashImage>(requestUri);
//             return unsplashImage?.Urls?.Regular;
//         }
//     }
// }
