using Forecastly.Api.Models;
using System.Diagnostics.Metrics;
using System.Text.Json;

namespace Forecastly.Api.Services
{
    public interface IWeatherService
    {
        Task<WeatherDTO> GetWeatherAsync(string cityName);
    }
}
