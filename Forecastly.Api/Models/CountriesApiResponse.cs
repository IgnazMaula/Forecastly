namespace Forecastly.Api.Models
{
    public class CountriesApiResponse
    {
        public bool Error { get; set; }
        public string Msg { get; set; }
        public List<CountryModel> Data { get; set; }
    }
}
