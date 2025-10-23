namespace Forecastly.Web.Models
{
    public class ApiSettings
    {
        public string BaseUrl { get; set; }
        public Endpoints Endpoints { get; set; }
    }

    public class Endpoints
    {
        public string Countries { get; set; }
        public string Cities { get; set; }
        public string Weather { get; set; }
    }
}