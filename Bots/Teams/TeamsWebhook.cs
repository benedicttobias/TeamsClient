using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Bots.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Bots.Teams
{
    public class TeamsWebhook
    {
        private readonly HttpClient _httpClient;
        private readonly DefaultContractResolver _defaultContractResolver;

        public TeamsWebhook(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("teams");

            _defaultContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };
        }

        public async Task<HttpStatusCode> Send(IWebhookMessage message)
        {
            var payloadJson = JsonConvert.SerializeObject(message, new JsonSerializerSettings
            {
                ContractResolver = _defaultContractResolver,
                Formatting = Formatting.Indented
            });
            
            _httpClient.DefaultRequestHeaders.Add("contentType", message.MediaType);
            var response = await _httpClient.PostAsync("", new StringContent(payloadJson));

            return response.StatusCode;
        }
    }
}
