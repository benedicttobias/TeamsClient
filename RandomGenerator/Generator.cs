using RestSharp;

namespace RandomGenerator
{
    public class Generator
    {
        private readonly RestClient _restClient;
        private string _baseUrl;

        public Generator(string baseUrl)
        {
            _baseUrl = baseUrl;
            _restClient = new RestClient(_baseUrl);
        }

        public string GetRandomParagraph(int paragraphCount)
        {
            var request = new RestRequest($"/{paragraphCount}/short");
            return _restClient.Get(request).Content;
        }
    }
}
