using System;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
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

        public async Task<HttpResponseMessage> SendAsync(IWebhookMessage message)
        {
            var webHookClient = new WebHookClient(_serviceUrl, message.MediaType);
            return await webHookClient.PostAsync(message);
        }
    }
}
