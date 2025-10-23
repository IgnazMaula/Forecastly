using Forecastly.Web.Models;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace Forecastly.Web.Services
{
    public class ApiService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;

        public ApiService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _config = config;
        }

        public async Task<List<CountryDTO>> GetCountriesAsync()
        {
            var baseUrl = _config["ApiSettings:BaseUrl"];
            var endpoint = _config["ApiSettings:Endpoints:Countries"];
            return await _http.GetFromJsonAsync<List<CountryDTO>>($"{baseUrl}{endpoint}");
        }

        public async Task<List<string>> GetCitiesAsync(string countryCode)
        {
            var baseUrl = _config["ApiSettings:BaseUrl"];
            var endpoint = _config["ApiSettings:Endpoints:Cities"].Replace("{countryCode}", countryCode);
            return await _http.GetFromJsonAsync<List<string>>($"{baseUrl}{endpoint}");
        }

        public async Task<WeatherDTO> GetWeatherAsync(string cityName)
        {
            var baseUrl = _config["ApiSettings:BaseUrl"];
            var endpoint = _config["ApiSettings:Endpoints:Weather"].Replace("{cityName}", cityName);
            return await _http.GetFromJsonAsync<WeatherDTO>($"{baseUrl}{endpoint}");
        }
    }
}
