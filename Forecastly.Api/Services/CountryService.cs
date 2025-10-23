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

        private List<CountryModel> Countries = new();

        public CountryService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        // Fetch data from countriesnow API
        private async Task GetCountriesAndCities()
        {
            if (Countries.Any()) return;

            string url = _configuration["ContriesNow:BaseUrl"];

            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<CountriesApiResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (!apiResponse.Error && apiResponse.Data != null)
                {
                    Countries = apiResponse.Data;
                }
            }
            catch (Exception ex)
            {
                // Fetch data from offline data if api failing
                string offlineFilePath = Path.Combine(AppContext.BaseDirectory, "Data", "countries-offline.json");

                var offlineJson = await File.ReadAllTextAsync(offlineFilePath);
                var offlineData = JsonSerializer.Deserialize<CountriesApiResponse>(offlineJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (offlineData != null && offlineData.Data != null)
                {
                    Countries = offlineData.Data;
                }
            }

        }

        public async Task<List<CountryDTO>> GetCountriesAsync()
        {
            await GetCountriesAndCities();

            return Countries
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
            await GetCountriesAndCities();
            var country = Countries.FirstOrDefault(c =>
                c.Iso2.Equals(countryCode, StringComparison.OrdinalIgnoreCase));
            return country?.Cities ?? new List<string>();
        }
    }
}
