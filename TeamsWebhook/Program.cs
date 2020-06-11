using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Bots.Models;
using Bots.Models.Weather;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UltimateTemperatureLibrary;


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

                var sections = new List<Section>();
                sections.Add(new Section
                {
                    ActivityText = randomGenerator,
                    ActivityImage = $"https://picsum.photos/200",
                });

                if (forecast.Consolidated_weather.Any())
                {
                    foreach (var weather in forecast.Consolidated_weather)
                    {
                        var maxTemp = new Celsius(weather.max_temp);
                        var minTemp = new Celsius(weather.min_temp);

                        sections.Add(new Section
                        {
                            ActivityText = $"Day: {weather.applicable_date} - Max: {maxTemp.ToFahrenheit().ToString("F0")} - Min: {minTemp.ToFahrenheit().ToString("F0")}"
                        });
                    }
                }
                

                var content = new CardConnector
                {
                    Title = "You have a new notification!",
                    Summary = "Subtitle",
                    Sections = sections,
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

                var teamsResponse = teamsWebhook.Send(content);
                logger.LogInformation($"Teams response: {teamsResponse}");
            }


            Console.ReadLine();
            return 0;
        }
    }
}
