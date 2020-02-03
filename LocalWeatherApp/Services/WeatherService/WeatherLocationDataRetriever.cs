using LocalWeatherApp.Services.WeatherService.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace LocalWeatherApp.Services.WeatherService
{
    public interface IWeatherLocationDataRetriever:  IDataRetriever<WeatherLocation>
    {
    }

    public class WeatherLocationDataRetriever : IWeatherLocationDataRetriever
    {
        private readonly List<WeatherLocation> items = new List<WeatherLocation>();

        private readonly IWeatherServiceClient weatherServiceClient;

        public WeatherLocationDataRetriever(IWeatherServiceClient weatherServiceClient)
        {
            this.weatherServiceClient = weatherServiceClient;
        }

        public async Task<WeatherLocation> GetItemAsync(string id)
        {
            return await Task.FromResult(this.items.FirstOrDefault(s => s.Woeid.ToString() == id));
        }

        public async Task<IEnumerable<WeatherLocation>> GetItemsAsync(bool forceRefresh = false)
        {
            var location = await this.GetLocation();
            var weatherLocationSearches = await this.weatherServiceClient.GetWeatherLocations(location.Latitude, location.Longitude);
            this.items.Clear();
            this.items.AddRange(weatherLocationSearches);
            return await Task.FromResult(this.items);
        }

        private async Task<Location> GetLocation()
        {
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();

                if (location != null)
                {
                    Debug.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                    return location;
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                Debug.WriteLine(fnsEx);
            }
            catch (FeatureNotEnabledException fneEx)
            {
                Debug.WriteLine(fneEx);
            }
            catch (PermissionException pEx)
            {
                Debug.WriteLine(pEx);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return null;
        }
    }
}