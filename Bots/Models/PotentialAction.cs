using System.Collections.Generic;
using Newtonsoft.Json;

namespace Bots.Models
{
    public class Potentialaction
    {
        public string Context { get; set; }
        [JsonProperty(PropertyName = "@type")]
        public string Type { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> Target { get; set; }

        public Potentialaction()
        {
            Target = new List<string>();
        }
    }
}
