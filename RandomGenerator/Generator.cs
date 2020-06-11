using System.Net.Http;
using System.Threading.Tasks;

namespace RandomGenerator
{
    public class Generator
    {
        private readonly HttpClient _httpClient;

        public Generator(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("randomGenerator");
        }

        public async Task<string> GetRandomParagraph(int paragraphCount)
        {
            
            return await _httpClient.GetStringAsync($"{paragraphCount}/short");
        }
    }
}
