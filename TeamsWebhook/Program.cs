using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Bots.Models;
using Bots.Models.Weather;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Exception = System.Exception;

namespace TeamsWebhook
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var serviceUrl = Environment.GetEnvironmentVariable("serviceUrl", EnvironmentVariableTarget.User) ?? string.Empty;

            // Create host so I can use services
            var builder = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHttpClient();
                    services.AddHttpClient("meta", c =>
                    {
                        c.BaseAddress = new Uri("https://www.metaweather.com/api/");
                    });
                    services.AddHttpClient("randomGenerator", c =>
                    {
                        c.BaseAddress = new Uri("https://loripsum.net/api/");
                    });
                    services.AddHttpClient("teams", c =>
                    {
                        c.BaseAddress = new Uri(serviceUrl);
                    });
                })
                .ConfigureLogging(logging =>
                {
                    logging.AddConsole();
                })
                .UseConsoleLifetime();

            var host = builder.Build();


            // Begin of the service scope
            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();

                logger.LogError("Log error test");

                var httpClientFactory = services.GetRequiredService<IHttpClientFactory>();
                var client = httpClientFactory.CreateClient("meta");


                var forecast = await client.GetFromJsonAsync<WeatherForecastModel>("location/2357024/");
                var jsonResponse = JsonConvert.SerializeObject(forecast);

                logger.LogInformation($"Weather response: {jsonResponse}");

                var teamsWebhook = new Bots.Teams.TeamsWebhook(httpClientFactory);

                var randomGenerator = await new RandomGenerator.Generator(httpClientFactory).GetRandomParagraph(3);

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
            }


            Console.ReadLine();
            return 0;
        }
    }
}
