using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace LocalWeatherApp.Configuration
{
    public class AppSettingsManager
    {
        private static AppSettingsManager _instance;
        private readonly JObject _secrets;

        private const string Namespace = "LocalWeatherApp";
        private const string FileName = "appsettings.json";

        private AppSettingsManager()
        {
            try
            {
                var assembly = typeof(AppSettingsManager).GetTypeInfo().Assembly;
                var stream = assembly.GetManifestResourceStream($"{Namespace}.{FileName}");
                using (var reader = new StreamReader(stream))
                {
                    var json = reader.ReadToEnd();
                    this._secrets = JObject.Parse(json);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to load secrets file, exception:{ex}");
            }
        }

        public static AppSettingsManager Settings => _instance ?? (_instance = new AppSettingsManager());

        public string this[string name]
        {
            get
            {
                try
                {
                    var path = name.Split(':');

                    var node = this._secrets[path[0]];
                    for (var index = 1; index < path.Length; index++)
                    {
                        node = node[path[index]];
                    }

                    return node.ToString();
                }
                catch (Exception)
                {
                    Debug.WriteLine($"Unable to retrieve secret '{name}'");
                    return string.Empty;
                }
            }
        }
    }
}

