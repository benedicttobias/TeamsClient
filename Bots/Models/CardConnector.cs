using System.Collections.Generic;

namespace Bots.Models
{
    public class CardConnector : IWebhookMessage
    {
        public string Type { get; set; }
        public string Context { get; set; }
        public string Summary { get; set; }
        public string Title { get; set; }
        public string MediaType { get; set; }
        public IEnumerable<Section> Sections { get; set; }
        public IEnumerable<Potentialaction> PotentialAction { get; set; }

        public CardConnector()
        {
            Sections = new List<Section>();
            PotentialAction = new List<Potentialaction>();
            MediaType = "application/vnd.microsoft.teams.card.o365connector";
        }
    }
}
