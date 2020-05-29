using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Bots.Teams
{
    public class WebHookClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _serviceUrl;
        private readonly string _mediaType;
        private readonly DefaultContractResolver _defaultContractResolver;

        public WebHookClient(string serviceUrl, string mediaType)
        {
            _mediaType = mediaType;
            _serviceUrl = serviceUrl;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));

            _defaultContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };
        }

        public async Task<HttpResponseMessage> PostAsync(object payload)
        {
            var payloadJson = JsonConvert.SerializeObject(payload, new JsonSerializerSettings
            {
                ContractResolver = _defaultContractResolver,
                Formatting = Formatting.Indented
            });

            var stringContent = new StringContent(payloadJson, Encoding.UTF8, _mediaType);

            return await _httpClient.PostAsync(_serviceUrl, stringContent);
        }
    }
}
