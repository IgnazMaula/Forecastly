using Forecastly.Web.Models;
using System.Net.Http.Json;

namespace Forecastly.Web.Services
{
    public class ApiService
    {
        private readonly HttpClient _http;

        public ApiService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<CountryDTO>> GetCountriesAsync()
        {
            return await _http.GetFromJsonAsync<List<CountryDTO>>("https://localhost:7070/api/Countries");
        }

        public async Task<List<string>> GetCitiesAsync(string countryCode)
        {
            return await _http.GetFromJsonAsync<List<string>>($"https://localhost:7070/api/Countries/{countryCode}/cities");
        }

        public async Task<WeatherDTO> GetWeatherAsync(string cityName)
        {
            return await _http.GetFromJsonAsync<WeatherDTO>($"https://localhost:7070/api/Weather/{cityName}");
        }
    }
}
