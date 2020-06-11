using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Model.Teams;
using UltimateTemperatureLibrary;
using Weather;


namespace TeamsWebhook
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            // Create host so I can use services
            var builder = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHttpClient();
                    services.AddHttpClient("weather", c =>
                    {
                        c.BaseAddress = new Uri("https://www.metaweather.com/api/");
                    });
                    services.AddHttpClient("randomGenerator", c =>
                    {
                        c.BaseAddress = new Uri("https://loripsum.net/api/");
                    });
                    services.AddHttpClient("teams", c =>
                    {
                        c.BaseAddress = new Uri(Environment.GetEnvironmentVariable("serviceUrl", EnvironmentVariableTarget.User) ?? string.Empty);
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
                var httpClientFactory = services.GetRequiredService<IHttpClientFactory>();
                
                
                var weatherClient = new WeatherClient(httpClientFactory);
                var teamsWebhook = new Bots.Teams.TeamsWebhook(httpClientFactory);
                var randomGeneratorClient = new RandomGenerator.Generator(httpClientFactory);
                
                var forecast = await weatherClient.GetWeatherForecastAsync(2357024);

                var sections = new List<Section>
                {
                    new Section
                    {
                        ActivityText = await randomGeneratorClient.GetRandomParagraph(3),
                        ActivityImage = $"https://picsum.photos/200",
                    }
                };

                if (forecast.Consolidated_weather.Any())
                {
                    foreach (var weather in forecast.Consolidated_weather)
                    {
                        var maxTemp = new Celsius(weather.max_temp);
                        var minTemp = new Celsius(weather.min_temp);

                        sections.Add(new Section
                        {
                            ActivityText = $"Date: {weather.applicable_date} - Max: {maxTemp.ToFahrenheit().ToString("F0")} - Min: {minTemp.ToFahrenheit().ToString("F0")}"
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
