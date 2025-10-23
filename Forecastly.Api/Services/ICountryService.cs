using Forecastly.Api.Models;
using System.Diagnostics.Metrics;
using System.Text.Json;

namespace Forecastly.Api.Services
{
    public interface ICountryService
    {
        Task<List<CountryDTO>> GetCountriesAsync();
        Task<List<string>> GetCitiesByCountryCodeAsync(string countryCode);
    }
}
