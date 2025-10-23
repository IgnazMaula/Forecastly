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
            // Use mocked response if API key is missing or cityName is empty
            if (string.IsNullOrEmpty(_configuration["OpenWeather:ApiKey"]) || string.IsNullOrWhiteSpace(cityName))
            {
                return GetMockedWeather(cityName);
            }

            // Call OpenWeatherMap
            try
            {
                string baseUrl = _configuration["OpenWeather:BaseUrl"];
                string apiKey = _configuration["OpenWeather:ApiKey"];
                string url = $"{baseUrl}?q={cityName}&appid={apiKey}&units=imperial";

                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Weather API error: {response.ReasonPhrase}");
                }

                var json = await response.Content.ReadAsStringAsync();
                var doc = JsonDocument.Parse(json);

                return ParseWeatherJson(doc);
            }
            catch (Exception ex)
            {
                throw new Exception($"Weather API error: {ex.Message}");
            }
        }

        internal WeatherDTO ParseWeatherJson(JsonDocument doc)
        {
            var main = doc.RootElement.GetProperty("main");
            var wind = doc.RootElement.GetProperty("wind");
            var sys = doc.RootElement.GetProperty("sys");
            var weatherArray = doc.RootElement.GetProperty("weather")[0];
            var clouds = doc.RootElement.GetProperty("clouds");
            var coord = doc.RootElement.GetProperty("coord");

            double tempF = main.GetProperty("temp").GetDouble();
            double humidity = main.GetProperty("humidity").GetDouble();

            return new WeatherDTO
            {
                City = doc.RootElement.GetProperty("name").GetString(),
                Country = sys.GetProperty("country").GetString(),
                TimeUtc = DateTime.UtcNow,

                // Temperature
                TempF = tempF,
                TempC = FahrenheitToCelsius(tempF),
                FeelsLikeF = main.GetProperty("feels_like").GetDouble(),
                FeelsLikeC = FahrenheitToCelsius(main.GetProperty("feels_like").GetDouble()),
                TempMinF = main.GetProperty("temp_min").GetDouble(),
                TempMinC = FahrenheitToCelsius(main.GetProperty("temp_min").GetDouble()),
                TempMaxF = main.GetProperty("temp_max").GetDouble(),
                TempMaxC = FahrenheitToCelsius(main.GetProperty("temp_max").GetDouble()),

                // Atmospheric data
                Pressure = main.GetProperty("pressure").GetInt32(),
                Humidity = (int)humidity,
                DewPoint = CalculateDewPoint(tempF, humidity),
                Visibility = doc.RootElement.GetProperty("visibility").GetInt32(),

                // Wind
                WindSpeed = wind.GetProperty("speed").GetDouble(),
                WindDirection = wind.GetProperty("deg").GetInt32(),

                // Weather conditions
                SkyMain = weatherArray.GetProperty("main").GetString(),
                SkyDescription = weatherArray.GetProperty("description").GetString(),
                Cloudiness = clouds.GetProperty("all").GetInt32(),
                Icon = weatherArray.GetProperty("icon").GetString(),

                // Sunrise / Sunset
                Sunrise = DateTimeOffset.FromUnixTimeSeconds(sys.GetProperty("sunrise").GetInt64()).UtcDateTime,
                Sunset = DateTimeOffset.FromUnixTimeSeconds(sys.GetProperty("sunset").GetInt64()).UtcDateTime,

                // Coordinates
                Longitude = coord.GetProperty("lon").GetDouble(),
                Latitude = coord.GetProperty("lat").GetDouble()
            };
        }

        internal WeatherDTO GetMockedWeather(string cityName)
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

        public double FahrenheitToCelsius(double tempF)
        {
            double tempC = (tempF - 32) * 5 / 9;
            return Math.Round(tempC, 2);
        }

        public double CalculateDewPoint(double tempF, double humidity)
        {
            double tempC = FahrenheitToCelsius(tempF);

            // Magnus formula constants
            double a = 17.27;
            double b = 237.7;

            double alpha = ((a * tempC) / (b + tempC)) + Math.Log(humidity / 100.0);
            double dewPoint = (b * alpha) / (a - alpha);

            return Math.Round(dewPoint, 2);
        }
    }
}
