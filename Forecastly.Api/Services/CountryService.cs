using Forecastly.Api.Models;
using Microsoft.Extensions.Configuration;
using System.Diagnostics.Metrics;
using System.Text.Json;

namespace Forecastly.Api.Services
{
    public class CountryService : ICountryService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        private List<CountryModel> _countriesCache = new();

        public CountryService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        // Fetch data from countriesnow API once
        private async Task EnsureDataLoadedAsync()
        {
            if (_countriesCache.Any()) return;

            string url = _configuration["ContriesNow:BaseUrl"];
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<CountriesApiResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (!apiResponse.Error && apiResponse.Data != null)
            {
                _countriesCache = apiResponse.Data;
            }
        }

        public async Task<List<CountryDTO>> GetCountriesAsync()
        {
            await EnsureDataLoadedAsync();

            return _countriesCache
                .Select(c => new CountryDTO
                {
                    Iso2 = c.Iso2,
                    Name = c.Country
                })
                .OrderBy(c => c.Name)
                .ToList();
        }

        public async Task<List<string>> GetCitiesByCountryCodeAsync(string countryCode)
        {
            await EnsureDataLoadedAsync();
            var country = _countriesCache.FirstOrDefault(c =>
                c.Iso2.Equals(countryCode, StringComparison.OrdinalIgnoreCase));
            return country?.Cities ?? new List<string>();
        }
    }
}
