using Forecastly.Web.Models;
using Forecastly.Web.Services;
using Microsoft.AspNetCore.Components;

namespace Forecastly.Web.Pages
{
    public partial class Home : ComponentBase
    {
        [Inject] protected ApiService ApiService { get; set; }
        [Inject] private ToastService Toast { get; set; }

        protected List<CountryDTO> Countries = new();
        protected List<string> Cities = new();
        protected WeatherDTO Weather;

        //protected CountryDTO? SelectedCountry { get; set; }
        //protected string? SelectedCity { get; set; }

        protected bool IsLoading { get; set; } = false;
        protected string? ErrorMessage { get; set; }

        private CountryDTO? _selectedCountry;
        protected CountryDTO? SelectedCountry
        {
            get => _selectedCountry;
            set
            {
                if (_selectedCountry == value) return;
                _selectedCountry = value;
                _ = SelectedCountryChangedAsync(value);
            }
        }

        private string? _selectedCity;
        protected string? SelectedCity
        {
            get => _selectedCity;
            set
            {
                if (_selectedCity == value) return;
                _selectedCity = value;
                _ = SelectedCityChangedAsync(value);
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await GetCountries();
        }

        private async Task GetCountries()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;

                Countries = await ApiService.GetCountriesAsync() ?? new List<CountryDTO>();
            }
            catch (Exception ex)
            {
                await Toast.ShowError($"Failed to load countries: {ex.Message}");
            }

            IsLoading = false;
        }

        protected Task<IEnumerable<CountryDTO>> SearchCountries(string searchText)
        {
            if (Countries == null || Countries.Count == 0)
                return Task.FromResult(Enumerable.Empty<CountryDTO>());

            if (string.IsNullOrWhiteSpace(searchText))
                return Task.FromResult(Countries.AsEnumerable());

            var filtered = Countries
                .Where(c => c.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(filtered);
        }

        private async Task SelectedCountryChangedAsync(CountryDTO? country)
        {
            if (country == null)
            {
                Cities.Clear();
                SelectedCity = null;
                Weather = null;
                StateHasChanged();
                return;
            }

            try
            {
                IsLoading = true;
                Cities = await ApiService.GetCitiesAsync(country.Iso2) ?? new List<string>();
                SelectedCity = null;
                Weather = null;
                StateHasChanged();
            }
            catch (Exception ex)
            {
                await Toast.ShowError($"Failed to load cities: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        protected Task<IEnumerable<string>> SearchCities(string searchText)
        {
            if (Cities == null || Cities.Count == 0)
                return Task.FromResult(Enumerable.Empty<string>());

            if (string.IsNullOrWhiteSpace(searchText))
                return Task.FromResult(Cities.AsEnumerable());

            var filtered = Cities.Where(c =>
                c.Contains(searchText, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(filtered);
        }

        private async Task SelectedCityChangedAsync(string? city)
        {
            if (string.IsNullOrEmpty(city)) return;

            try
            {
                IsLoading = true;
                var query = $"{city},{SelectedCountry.Iso2}";
                Weather = await ApiService.GetWeatherAsync(query);
                StateHasChanged();
            }
            catch (Exception ex)
            {
                await Toast.ShowError($"Failed to load weather data: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private string GetFlagUrl(string iso2)
        {
            return $"https://flagcdn.com/24x18/{iso2.ToLower()}.png";
        }

        private string GetFlagUrlWeather(string iso2)
        {
            return $"https://flagcdn.com/48x36/{iso2.ToLower()}.png";
        }

        private string GetWeatherBackgroundClass()
        {
            if (Weather == null)
                return "weather-container default-bg";

            return Weather.SkyMain.ToLower() switch
            {
                "clear" => "weather-container sunny-bg",
                "clouds" => "weather-container cloudy-bg",
                "rain" => "weather-container rainy-bg",
                "drizzle" => "weather-container drizzle-bg",
                "thunderstorm" => "weather-container stormy-bg",
                "snow" => "weather-container snowy-bg",
                "mist" or "fog" or "haze" => "weather-container foggy-bg",
                _ => "weather-container default-bg"
            };
        }

    }
}
