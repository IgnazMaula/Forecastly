using Forecastly.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Forecastly.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountriesController : ControllerBase
    {
        private readonly ICountryService _countryService;

        public CountriesController(ICountryService countryService)
        {
            _countryService = countryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCountries()
        {
            try
            {
                var countries = await _countryService.GetCountriesAsync();
                return Ok(countries);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{countryCode}/cities")]
        public async Task<IActionResult> GetCities(string countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
            {
                return BadRequest("Country code is required.");
            }
            try
            {
                var cities = await _countryService.GetCitiesByCountryCodeAsync(countryCode);

                if (cities == null || !cities.Any())
                {
                    return NotFound($"No cities found for country code '{countryCode}'.");
                }

                return Ok(cities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
