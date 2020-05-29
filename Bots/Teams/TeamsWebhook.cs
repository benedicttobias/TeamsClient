using System.Net;
using Bots.Models;

namespace Bots.Teams
{
    public class TeamsWebhook
    {
        private readonly string _serviceUrl;

        public TeamsWebhook(string serviceUrl)
        {
            _serviceUrl = serviceUrl;
        }

        public HttpStatusCode Send(IWebhookMessage message)
        {
            var webHookClient = new WebHookClient(_serviceUrl, message.MediaType);
            return webHookClient.Post(message);
        }
    }
}
