using LocalWeatherApp.Services.WeatherService.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocalWeatherApp.Services.WeatherService
{
    public interface IWeatherLocationDetailDataRetriever : IDataRetriever<WeatherLocationDetail>
    {
    }

    public class WeatherLocationDetailDataRetriever : IWeatherLocationDetailDataRetriever
    {
        private readonly IWeatherServiceClient weatherServiceClient;

        public WeatherLocationDetailDataRetriever(IWeatherServiceClient weatherServiceClient)
        {
            this.weatherServiceClient = weatherServiceClient;
        }

        public async Task<WeatherLocationDetail> GetItemAsync(string woeid)
        {
            var weatherLocation = await this.weatherServiceClient.GetWeatherLocationDetail(woeid);
            return weatherLocation;
        }

        public Task<IEnumerable<WeatherLocationDetail>> GetItemsAsync(bool forceRefresh = false)
        {
            throw new NotImplementedException();
        }
    }
}
