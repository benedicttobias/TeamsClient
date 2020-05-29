using System;
using System.Collections.Generic;
using Bots.Models;

namespace TeamsWebhook
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceUrl = Environment.GetEnvironmentVariable("serviceUrl", EnvironmentVariableTarget.User);

            var teamsWebhook = new Bots.Teams.TeamsWebhook(serviceUrl);

            var randomGenerator = new RandomGenerator.Generator("https://loripsum.net/api").GetRandomParagraph(3);

            var content = new CardConnector
            {
                Title = "You have a new notification!",
                Summary = "Subtitle",
                Sections = new[]
                {
                    new Section
                    {
                        ActivityText = randomGenerator,
                        ActivityImage = $"https://picsum.photos/200",
                    }
                },
                PotentialAction = new List<Potentialaction>
                {
                    new Potentialaction
                    {
                        Type = "ViewAction",
                        Name = "View image source",
                        Target = new List<string>
                        {
                            "https://picsum.photos/"
                        }
                    }
                }
            };

            var response = teamsWebhook.Send(content);
            Console.WriteLine(response);
            Console.ReadLine();
        }
    }
}
