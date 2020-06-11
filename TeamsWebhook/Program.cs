using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using Bots.Models;
using Bots.Models.Weather;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TeamsWebhook
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHttpClient();
                    
                })
                .ConfigureLogging(logging =>
                {
                    logging.AddConsole();
                })
                .UseConsoleLifetime();

            var host = builder.Build();

            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();

                logger.LogError("Log error test");
                try
                {
                    var _httpClientFactory = services.GetRequiredService<IHttpClientFactory>();

                    var request = new HttpRequestMessage(HttpMethod.Get, "https://www.metaweather.com/api/location/2357024/");

                    var client = _httpClientFactory.CreateClient();

                    var weatherResponse = await client.SendAsync(request);

                    if (weatherResponse.IsSuccessStatusCode)
                    {
                        var forecast = await weatherResponse.Content.ReadFromJsonAsync<WeatherForecastModel>();
                        logger.LogInformation($"@forecast", forecast);
                    }
                    else
                    {
                        logger.LogError($"Error on getting the forecast: {weatherResponse.ReasonPhrase}");
                    }
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Error on main");
                    throw;
                }
            }

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

            return 0;
        }
    }
}
