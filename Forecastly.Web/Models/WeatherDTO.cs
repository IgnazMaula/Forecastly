namespace Forecastly.Web.Models
{
    public class WeatherDTO
    {
        // Location
        public string City { get; set; }
        public string Country { get; set; }
        public DateTime TimeUtc { get; set; }

        // Coordinates
        public double Longitude { get; set; }
        public double Latitude { get; set; }

        // Temperature
        public double TempF { get; set; }
        public double TempC { get; set; }
        public double FeelsLikeF { get; set; }
        public double FeelsLikeC { get; set; }
        public double TempMinF { get; set; }
        public double TempMinC { get; set; }
        public double TempMaxF { get; set; }
        public double TempMaxC { get; set; }

        // Atmospheric data
        public int Pressure { get; set; }
        public int Humidity { get; set; }
        public double DewPoint { get; set; }
        public int Visibility { get; set; }

        // Wind
        public double WindSpeed { get; set; }
        public int WindDirection { get; set; }

        // Weather / Clouds
        public string SkyMain { get; set; }
        public string SkyDescription { get; set; }
        public int Cloudiness { get; set; }

        // Sun
        public DateTime Sunrise { get; set; }
        public DateTime Sunset { get; set; }
    }

}
