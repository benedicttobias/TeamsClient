using Bots.Models.Weather;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Weather
{
    public class WeatherClient
    {
        private readonly HttpClient _httpClient;

        public WeatherClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("weather");
        }

        public async Task<WeatherForecastModel> GetWeatherForecastAsync(int woeid)
        {
            return await _httpClient.GetFromJsonAsync<WeatherForecastModel>($"location/{woeid}/");
        }
    }
}
