using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Forecastly.Api.Services;
using Forecastly.Api.Tests.Helpers;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Forecastly.Api.Tests.Services
{
    public class WeatherServiceTests
    {
        private readonly WeatherService _service;

        public WeatherServiceTests()
        {
            _service = new WeatherService(null, null);
        }

        [Theory]
        [InlineData(32, 0)]
        [InlineData(212, 100)]
        [InlineData(68, 20)]
        public void FahrenheitToCelsius_ShouldConvertCorrectly(double f, double expectedC)
        {
            var result = _service.FahrenheitToCelsius(f);
            Assert.Equal(expectedC, result, 2);
        }

        [Theory]
        [InlineData(68, 50, 9.2)]
        [InlineData(86, 70, 23.9)]
        public void CalculateDewPoint_ShouldReturnCorrectValue(double tempF, double humidity, double expected)
        {
            var service = new WeatherService(null, null);
            var result = service.CalculateDewPoint(tempF, humidity);
            Assert.Equal(expected, result, 1);
        }

        [Fact]
        public async Task GetWeatherAsync_ShouldReturnWeatherDTO_WhenHttpSucceeds()
        {
            var json = @"{
                ""name"": ""Sydney"",
                ""sys"": {""country"": ""AU"", ""sunrise"": 1, ""sunset"": 2},
                ""main"": {""temp"": 68, ""feels_like"": 70, ""temp_min"": 65, ""temp_max"": 72, ""pressure"": 1012, ""humidity"": 50},
                ""visibility"": 10000,
                ""wind"": {""speed"": 5, ""deg"": 180},
                ""weather"": [{""main"": ""Clear"", ""description"": ""clear sky"", ""icon"": ""01d""}],
                ""clouds"": {""all"": 0},
                ""coord"": {""lon"": 0, ""lat"": 0}
            }";

            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json)
            };
            var httpClient = new HttpClient(new FakeHttpMessageHandler(httpResponse));

            var inMemorySettings = new Dictionary<string, string> { { "OpenWeather:ApiKey", "test" }, { "OpenWeather:BaseUrl", "https://api.openweathermap.org/data/2.5/weather" } };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();

            var service = new WeatherService(httpClient, configuration);

            var result = await service.GetWeatherAsync("Sydney");

            Assert.Equal("Sydney", result.City);
            Assert.Equal("AU", result.Country);
            Assert.Equal("Clear", result.SkyMain);
            Assert.Equal(68, result.TempF);
            Assert.Equal(20, result.TempC);
        }

        [Fact]
        public async Task GetWeatherAsync_ShouldReturnMockedWeather_WhenHttpFails()
        {
            var httpResponse = new HttpResponseMessage(HttpStatusCode.BadRequest); // Simulate failure
            var httpClient = new HttpClient(new FakeHttpMessageHandler(httpResponse));

            var inMemorySettings = new Dictionary<string, string>();
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();

            var service = new WeatherService(httpClient, configuration);

            var result = await service.GetWeatherAsync("Paris");

            Assert.Equal("Paris", result.City);
            Assert.Equal("US", result.Country);
        }

    }
}
