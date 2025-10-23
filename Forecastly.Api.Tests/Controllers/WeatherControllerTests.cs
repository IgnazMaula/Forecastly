using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Forecastly.Api.Controllers;
using Forecastly.Api.Services;
using Forecastly.Api.Models;
using Forecastly.Api.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Forecastly.Api.Tests.Controllers
{
    public class WeatherServiceTests
    {
        [Fact]
        public async Task GetWeather_ShouldReturnBadRequest_WhenCityIsEmpty()
        {
            var mockService = new Mock<IWeatherService>();
            var controller = new WeatherController(mockService.Object);

            var result = await controller.GetWeather("");

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetWeather_ShouldReturnOk_WhenServiceReturnsWeather()
        {
            var mockService = new Mock<IWeatherService>();
            mockService.Setup(s => s.GetWeatherAsync("Sydney")).ReturnsAsync(new WeatherDTO { City = "Sydney", Country = "AU" });

            var controller = new WeatherController(mockService.Object);

            var result = await controller.GetWeather("Sydney");

            var okResult = Assert.IsType<OkObjectResult>(result);
            var weather = Assert.IsType<WeatherDTO>(okResult.Value);
            Assert.Equal("Sydney", weather.City);
        }

        [Fact]
        public async Task GetWeather_ShouldReturn500_WhenServiceThrows()
        {
            var mockService = new Mock<IWeatherService>();
            mockService.Setup(s => s.GetWeatherAsync("Sydney")).ThrowsAsync(new Exception("API failed"));

            var controller = new WeatherController(mockService.Object);

            var result = await controller.GetWeather("Sydney");

            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusResult.StatusCode);
        }

    }
}
