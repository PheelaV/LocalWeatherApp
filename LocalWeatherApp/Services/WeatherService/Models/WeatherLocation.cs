namespace LocalWeatherApp.Services.WeatherService.Models
{
    using Newtonsoft.Json;

    public class WeatherLocation
    {
        [JsonProperty("distance")]
        public long Distance { get; set; }

        [JsonProperty("title"), JsonRequired]
        public string Title { get; set; }

        [JsonProperty("location_type")]
        public string LocationType { get; set; }

        [JsonProperty("woeid"), JsonRequired]
        public int Woeid { get; set; }

        [JsonProperty("latt_long")]
        public string LattLong { get; set; }
    }
}
