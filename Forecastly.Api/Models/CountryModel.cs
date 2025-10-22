namespace Forecastly.Api.Models
{
    public class CountryModel
    {
        public string Iso2 { get; set; }
        public string Iso3 { get; set; }
        public string Country { get; set; }
        public List<string> Cities { get; set; } = new();
    }
}
