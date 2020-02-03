using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LocalWeatherApp.Services.HttpService
{
    public interface IAppHttpClient
    {
        string BaseUrl { get; set; }
    }
    public class AppHttpClient : IAppHttpClient
    {
        public string BaseUrl
        {
            get => this._baseUrl;
            set => this._baseUrl = value?.TrimEnd('/');
        }

        protected const string QUALITY_HEADER_APPLICATION_JSON = "application/json";

        protected HttpClient _httpClient;
        protected JsonSerializerSettings JsonSerializerSettings => this._settings.Value;

        private Lazy<JsonSerializerSettings> _settings;
        private string _baseUrl;

        /// <summary>
        /// Use this constructor for optimal performance, Xamarin will decide at run-time depending on the
        /// specific platform it is running on, which native implementation of HttpClient to use.
        /// </summary>
        public AppHttpClient(IHttpClientFactory httpClientFactory)
        {
            var httpClient = httpClientFactory.CreateClient();
            this.Init(httpClient);
        }

        /// <summary>
        /// WARNING! Only use for unit tests, for optimal performance and rich feature support always use the baseAddress constructor.
        /// </summary>
        /// <param name="messageHandler">Mock httpClientHandler</param>
        public AppHttpClient(HttpMessageHandler messageHandler)
        {
#if (!TEST)
            throw new InvalidEnvironmentException("Not running in TEST environment.");
#endif
            var httpClient = new HttpClient(messageHandler);
            this.Init(httpClient);
        }

        private void Init(HttpClient httpClient)
        {
            this._httpClient = httpClient;
            this._settings = new Lazy<JsonSerializerSettings>(() =>
            {
                var settings = new JsonSerializerSettings();
                return settings;
            });
        }


        protected async Task<ApiException<ProblemDetails>> NegativeSuccessCodeResult(string message, HttpResponseMessage response, IReadOnlyDictionary<string, IEnumerable<string>> headers)
        {
            var objectResponse_ = await this.ReadObjectResponseAsync<ProblemDetails>(response, headers)
                .ConfigureAwait(false);
            return new ApiException<ProblemDetails>(message, (int)response.StatusCode,
                objectResponse_.Text, headers, objectResponse_.Object, null);
        }

        protected async Task<T> PositiveSuccessCodeResult<T>(HttpResponseMessage response, Dictionary<string, IEnumerable<string>> headers)
        {
            var objectResponse_ = await this.ReadObjectResponseAsync<T>(response, headers)
                .ConfigureAwait(false);
            return objectResponse_.Object;
        }

        protected virtual async Task<ObjectResponseResult<T>> ReadObjectResponseAsync<T>(HttpResponseMessage response, IReadOnlyDictionary<string, IEnumerable<string>> headers)
        {
            if (response?.Content == null)
            {
                return new ObjectResponseResult<T>(default, string.Empty);
            }

            try
            {
                using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                using (var streamReader = new StreamReader(responseStream))
                using (var jsonTextReader = new JsonTextReader(streamReader))
                {
                    var serializer = JsonSerializer.Create(this.JsonSerializerSettings);
                    var typedBody = serializer.Deserialize<T>(jsonTextReader);
                    return new ObjectResponseResult<T>(typedBody, string.Empty);
                }
            }
            catch (JsonException exception)
            {
                var message = "Could not deserialize the response body stream as " + typeof(T).FullName + ".";
                Debug.WriteLine(message);
                throw new ApiException(message, (int)response.StatusCode, string.Empty, headers, exception);
            }
        }

        protected string ConvertToString(object value)
        {
            return this.ConvertToString(value, CultureInfo.InvariantCulture);
        }
        protected string ConvertToString(object value, IFormatProvider cultureInfo)
        {
            switch (value)
            {
                case string stringValue:
                    return stringValue;
                case Enum _:
                    {
                        var name = Enum.GetName(value.GetType(), value);

                        if (name == null)
                        {
                            return Convert.ToString(value, cultureInfo);
                        }

                        var field = value.GetType().GetTypeInfo().GetDeclaredField(name);

                        if (field == null)
                        {
                            return Convert.ToString(value, cultureInfo);
                        }

                        if (field.GetCustomAttribute(typeof(EnumMemberAttribute)) is EnumMemberAttribute attribute)
                        {
                            return attribute.Value ?? name;
                        }

                        break;
                    }

                case bool _:
                    return Convert.ToString(value, cultureInfo)?.ToLowerInvariant();
                case byte[] bytes:
                    return Convert.ToBase64String(bytes);
                default:
                    {
                        if (value != null && value.GetType().IsArray)
                        {
                            var array = ((Array)value).OfType<object>();
                            return string.Join(",", array.Select(o => this.ConvertToString(o, cultureInfo)));
                        }

                        break;
                    }
            }

            return Convert.ToString(value, cultureInfo);
        }

        protected async Task<HttpResponseMessage> SendRequest(HttpRequestMessage request, string url, HttpMethod method, CancellationToken cancellationToken, object content = null)
        {
            if (content != null)
            {
                request.Content = new StringContent(JsonConvert.SerializeObject(content, this._settings.Value));
                request.Content.Headers.ContentType.MediaType = "application/json";
            }
            request.Method = method;
            request.Headers.Accept.Add(
                MediaTypeWithQualityHeaderValue.Parse(QUALITY_HEADER_APPLICATION_JSON));
            request.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

            return await this._httpClient.SendAsync
            (
                request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken
            ).ConfigureAwait(false);
        }

        protected Dictionary<string, IEnumerable<string>> ExtractHeaders(HttpResponseMessage response)
        {
            var headers = new Dictionary<string, IEnumerable<string>>();

            if (response.Content?.Headers == null) return headers;

            foreach (var item_ in response.Content.Headers)
                headers[item_.Key] = item_.Value;

            return headers;
        }

        protected struct ObjectResponseResult<T>
        {
            public ObjectResponseResult(T responseObject, string responseText)
            {
                this.Object = responseObject;
                this.Text = responseText;
            }

            public T Object { get; }

            public string Text { get; }
        }

        protected class ApiException : Exception
        {
            public int StatusCode { get; }

            public string Response { get; }

            public IReadOnlyDictionary<string, IEnumerable<string>> Headers { get; }

            public ApiException(string message, int statusCode, string response, IReadOnlyDictionary<string, IEnumerable<string>> headers, Exception innerException)
                : base(message + "\n\nStatus: " + statusCode + "\nResponse: \n" + response.Substring(0, response.Length >= 512 ? 512 : response.Length), innerException)
            {
                this.StatusCode = statusCode;
                this.Response = response;
                this.Headers = headers;
            }

            public override string ToString()
            {
                return $"HTTP Response: \n\n{this.Response}\n\n{base.ToString()}";
            }
        }

        protected class ApiException<TResult> : ApiException
        {
            public TResult Result { get; }

            public ApiException(string message, int statusCode, string response, IReadOnlyDictionary<string, IEnumerable<string>> headers, TResult result, Exception innerException)
                : base(message, statusCode, response, headers, innerException)
            {
                this.Result = result;
            }
        }

        protected class ProblemDetails
        {
            [JsonProperty("type", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string Type { get; set; }

            [JsonProperty("title", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string Title { get; set; }

            [JsonProperty("status", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public int? Status { get; set; }

            [JsonProperty("detail", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string Detail { get; set; }

            [JsonProperty("instance", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
            public string Instance { get; set; }

            [JsonExtensionData]
            public IDictionary<string, object> AdditionalProperties { get; set; } = new Dictionary<string, object>();
        }

        protected class InvalidEnvironmentException : Exception
        {
            public InvalidEnvironmentException(string message): base(message)
            {
                
            }
        }
    }
}
