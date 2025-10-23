using Forecastly.Api.Models;
using System.Diagnostics.Metrics;
using System.Text.Json;

namespace Forecastly.Api.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public WeatherService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<WeatherDTO> GetWeatherAsync(string cityName)
        {
            // Option 1: Mocked response for offline/testing
            if (string.IsNullOrEmpty(_configuration["OpenWeather:ApiKey"]))
            {
                return GetMockedWeather(cityName);
            }

            // Option 2: Call OpenWeatherMap
            string baseUrl = _configuration["OpenWeather:BaseUrl"];
            string apiKey = _configuration["OpenWeather:ApiKey"];
            string url = $"{baseUrl}?q={cityName}&appid={apiKey}&units=imperial";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Weather API error: {response.ReasonPhrase}");

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);

            var info = new WeatherDTO
            {
                City = doc.RootElement.GetProperty("name").GetString(),
                Country = doc.RootElement.GetProperty("sys").GetProperty("country").GetString(),
                TimeUtc = DateTime.UtcNow,

                // Temperature
                TempF = doc.RootElement.GetProperty("main").GetProperty("temp").GetDouble(),
                TempC = FahrenheitToCelsius(doc.RootElement.GetProperty("main").GetProperty("temp").GetDouble()),
                FeelsLikeF = doc.RootElement.GetProperty("main").GetProperty("feels_like").GetDouble(),
                FeelsLikeC = FahrenheitToCelsius(doc.RootElement.GetProperty("main").GetProperty("feels_like").GetDouble()),
                TempMinF = doc.RootElement.GetProperty("main").GetProperty("temp_min").GetDouble(),
                TempMinC = FahrenheitToCelsius(doc.RootElement.GetProperty("main").GetProperty("temp_min").GetDouble()),
                TempMaxF = doc.RootElement.GetProperty("main").GetProperty("temp_max").GetDouble(),
                TempMaxC = FahrenheitToCelsius(doc.RootElement.GetProperty("main").GetProperty("temp_max").GetDouble()),

                // Atmospheric data
                Pressure = doc.RootElement.GetProperty("main").GetProperty("pressure").GetInt32(),
                Humidity = doc.RootElement.GetProperty("main").GetProperty("humidity").GetInt32(),
                DewPoint = CalculateDewPoint(
                    doc.RootElement.GetProperty("main").GetProperty("temp").GetDouble(),
                    doc.RootElement.GetProperty("main").GetProperty("humidity").GetDouble()
                ),
                Visibility = doc.RootElement.GetProperty("visibility").GetInt32(),

                // Wind
                WindSpeed = doc.RootElement.GetProperty("wind").GetProperty("speed").GetDouble(),
                WindDirection = doc.RootElement.GetProperty("wind").GetProperty("deg").GetInt32(),

                // Weather conditions
                SkyMain = doc.RootElement.GetProperty("weather")[0].GetProperty("main").GetString(),
                SkyDescription = doc.RootElement.GetProperty("weather")[0].GetProperty("description").GetString(),
                Cloudiness = doc.RootElement.GetProperty("clouds").GetProperty("all").GetInt32(),
                Icon = doc.RootElement.GetProperty("weather")[0].GetProperty("icon").GetString(),

                // Sunrise / Sunset
                Sunrise = DateTimeOffset.FromUnixTimeSeconds(doc.RootElement.GetProperty("sys").GetProperty("sunrise").GetInt64()).UtcDateTime,
                Sunset = DateTimeOffset.FromUnixTimeSeconds(doc.RootElement.GetProperty("sys").GetProperty("sunset").GetInt64()).UtcDateTime,

                // Coordinates
                Longitude = doc.RootElement.GetProperty("coord").GetProperty("lon").GetDouble(),
                Latitude = doc.RootElement.GetProperty("coord").GetProperty("lat").GetDouble()
            };

            return info;
        }

        private WeatherDTO GetMockedWeather(string cityName)
        {
            double tempF = 75;
            double humidity = 50;

            return new WeatherDTO
            {
                City = cityName,
                Country = "US",
                TimeUtc = DateTime.UtcNow,

                // Temperature
                TempF = tempF,
                TempC = FahrenheitToCelsius(tempF),
                FeelsLikeF = 77,
                FeelsLikeC = FahrenheitToCelsius(77),
                TempMinF = 70,
                TempMinC = FahrenheitToCelsius(70),
                TempMaxF = 80,
                TempMaxC = FahrenheitToCelsius(80),

                // Atmospheric data
                Pressure = 1012,
                Humidity = (int)humidity,
                DewPoint = CalculateDewPoint(tempF, humidity),
                Visibility = 10000,

                // Wind
                WindSpeed = 5,
                WindDirection = 180,

                // Weather conditions
                SkyMain = "Clear",
                SkyDescription = "clear sky",
                Cloudiness = 0,

                // Sunrise / Sunset
                Sunrise = DateTime.UtcNow.AddHours(-6),
                Sunset = DateTime.UtcNow.AddHours(6),

                // Coordinates
                Longitude = 0,
                Latitude = 0
            };
        }

        private double FahrenheitToCelsius(double tempF)
        {
            double tempC = (tempF - 32) * 5 / 9;
            return Math.Round(tempC, 2);
        }

        private double CalculateDewPoint(double tempF, double humidity)
        {
            double tempC = FahrenheitToCelsius(tempF);
            double dewPoint = tempC - ((100 - humidity) / 5.0);
            return Math.Round(dewPoint, 2);
        }
    }
}
