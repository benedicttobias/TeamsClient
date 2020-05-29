using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;

namespace Bots.Teams
{
    public class WebHookClient
    {
        private readonly RestClient _restClient;
        private readonly string _serviceUrl;
        private readonly DefaultContractResolver _defaultContractResolver;

        public WebHookClient(string serviceUrl, string mediaType)
        {
            _serviceUrl = serviceUrl;
            _restClient = new RestClient(_serviceUrl);
            _restClient.AddDefaultHeaders(new Dictionary<string, string>
            {
                {"contentType", mediaType} 
            });

            _defaultContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };
        }

        public HttpStatusCode Post(object payload)
        {
            var payloadJson = JsonConvert.SerializeObject(payload, new JsonSerializerSettings
            {
                ContractResolver = _defaultContractResolver,
                Formatting = Formatting.Indented
            });

            var request = new RestRequest(Method.POST);
            request.AddJsonBody(payloadJson);

            return _restClient.Execute(request).StatusCode;
        }
    }
}
