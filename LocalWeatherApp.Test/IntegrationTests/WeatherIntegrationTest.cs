using System.Net.Http;
using LocalWeatherApp.Services.WeatherService;
using Xunit;

namespace LocalWeatherApp.Test.IntegrationTests
{
    public class MockHttpClientFactory : IHttpClientFactory 
    {
        public HttpClient CreateClient(string name)
        {
            return  new HttpClient();
        }
    }
    public class WeatherIntegrationTest
    {
        private const string HonoluluWoeid = "2423945";
        private const double Latitude = 21.304850;
        private const double Longitude = -157.857758;

        [Fact]
        public async void GetWeatherLocations()
        {
            var factory = new MockHttpClientFactory();
            var client = new WeatherServiceClient(factory);

            var result = await client.GetWeatherLocations(Latitude, Longitude);
        }

        [Fact]
        public async void GetWeatherLocationDetail()
        {
            var factory = new MockHttpClientFactory();
            var client = new WeatherServiceClient(factory);

            var result = await client.GetWeatherLocationDetail(HonoluluWoeid);
        }
    }
}
