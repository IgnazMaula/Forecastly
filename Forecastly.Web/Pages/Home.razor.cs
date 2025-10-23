using Forecastly.Web.Models;
using Forecastly.Web.Services;
using Microsoft.AspNetCore.Components;

namespace Forecastly.Web.Pages
{
    public partial class Home : ComponentBase
    {
        [Inject] protected ApiService ApiService { get; set; }

        protected List<CountryDTO> Countries = new();
        protected List<string> Cities = new();
        protected WeatherDTO Weather;

        protected override async Task OnInitializedAsync()
        {
            Countries = await ApiService.GetCountriesAsync();
        }

        protected async Task OnCountryChanged(ChangeEventArgs e)
        {
            var countryCode = e.Value?.ToString();
            if (!string.IsNullOrEmpty(countryCode))
            {
                Cities = await ApiService.GetCitiesAsync(countryCode);
                Weather = null;
            }
            else
            {
                Cities.Clear();
                Weather = null;
            }
        }

        protected async Task OnCityChanged(ChangeEventArgs e)
        {
            var cityName = e.Value?.ToString();
            if (!string.IsNullOrEmpty(cityName))
            {
                Weather = await ApiService.GetWeatherAsync(cityName);
            }
        }
    }

}
