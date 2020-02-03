using System;
using System.Diagnostics;
using LocalWeatherApp.Configuration;
using LocalWeatherApp.Services.HttpService;
using LocalWeatherApp.Services.WeatherService.Models;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LocalWeatherApp.Services.WeatherService
{
    public interface IWeatherServiceClient
    {
        /// <summary>
        /// Gets the list of weather location searches based on coordinates.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns>A list </returns>
        Task<WeatherLocation[]> GetWeatherLocations(double latitude, double longitude);
        /// <summary>
        /// Gets a concrete weather location with all of its details.
        /// </summary>
        /// <param name="woeid"></param>
        /// <returns></returns>
        Task<WeatherLocationDetail> GetWeatherLocationDetail(string woeid);
    }

    public class WeatherServiceClient : AppHttpClient, IWeatherServiceClient
    {
        private const string LOCATION = "/api/location";
        /// <summary>
        /// latitude and longitude
        /// </summary>
        private const string LOCATION_SEARCH_EP = "/search/?lattlong={0},{1}";

        /// <summary>
        /// woeid - Where On Earth ID
        /// </summary>
        private const string LOCATION_GET_EP = "/{0}";

        /// <inheritdoc/>
        public WeatherServiceClient(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
            this.Init();
        }

        /// <inheritdoc/>
        public WeatherServiceClient(HttpMessageHandler messageHandler) : base(messageHandler)
        {
            this.Init();
        }

        private void Init()
        {
            this.BaseUrl = AppSettingsManager.Settings["WeatherService"];
        }

        public async Task<WeatherLocation[]> GetWeatherLocations(double latitude, double longitude)
        {
            var urlBuilder_ = new StringBuilder();
            urlBuilder_.Append(this.BaseUrl != null ? this.BaseUrl.TrimEnd('/') : "")
                .Append(LOCATION)
                .Append(string.Format(LOCATION_SEARCH_EP,
                    this.ConvertToString(latitude, CultureInfo.InvariantCulture),
                    this.ConvertToString(longitude, CultureInfo.InvariantCulture)));

            using (var request_ = new HttpRequestMessage())
            {
                var response_ = await this.SendRequest(request_, urlBuilder_.ToString(), HttpMethod.Get, CancellationToken.None);
                try
                {
                    var headers_ = this.ExtractHeaders(response_);

                    var statusCode = (int) response_.StatusCode;
                    switch (statusCode)
                    {
                        case 200: 
                            return await Task.Run(async () =>
                                await this.PositiveSuccessCodeResult<WeatherLocation[]>(response_, headers_));
                        case 400:
                            throw await this.NegativeSuccessCodeResult("Bad request", response_, headers_);
                        case 404:
                            throw await this.NegativeSuccessCodeResult("Not found", response_, headers_);
                        default:
                        {
                            if (statusCode != 200 && statusCode != 204)
                            {
                                var message = "The HTTP status code of the response was not expected (" + statusCode +
                                              ").";

                                throw await this.NegativeSuccessCodeResult(message, response_, headers_);
                            }

                            break;
                        }
                    }

                    return default;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    throw e;
                }
                finally
                {
                    response_?.Dispose();
                }
            }
        }

        public async Task<WeatherLocationDetail> GetWeatherLocationDetail(string woeid)
        {
            var urlBuilder_ = new StringBuilder();
            urlBuilder_.Append(this.BaseUrl != null ? this.BaseUrl.TrimEnd('/') : "")
                .Append(LOCATION)
                .Append(string.Format(LOCATION_GET_EP, woeid));

            using (var request_ = new HttpRequestMessage())
            {
                var response_ = await this.SendRequest(request_, urlBuilder_.ToString(), HttpMethod.Get, CancellationToken.None);
                try
                {
                    var headers_ = this.ExtractHeaders(response_);

                    var statusCode = (int)response_.StatusCode;
                    switch (statusCode)
                    {
                        case 200:
                            return await Task.Run(async () =>
                                await this.PositiveSuccessCodeResult<WeatherLocationDetail>(response_, headers_));
                        case 400:
                            throw await this.NegativeSuccessCodeResult("Bad request", response_, headers_);
                        case 404:
                            throw await this.NegativeSuccessCodeResult("Not found", response_, headers_);
                        default:
                        {
                            if (statusCode != 200 && statusCode != 204)
                            {
                                var message = "The HTTP status code of the response was not expected (" + statusCode + ").";

                                throw await this.NegativeSuccessCodeResult(message, response_, headers_);
                            }

                            break;
                        }
                    }

                    return default;
                }
                finally
                {
                    response_?.Dispose();
                }
            }
        }


    }
}
