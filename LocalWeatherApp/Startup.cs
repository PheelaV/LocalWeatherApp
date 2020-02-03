//TODO: Postponing the usage of Microsoft.Extensions.Hosting until phase 1 is finished
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using LocalWeatherApp.Services.WeatherService;
using LocalWeatherApp.ViewModels.Weather;
using LocalWeatherApp.Views;
using Xamarin.Essentials;

namespace LocalWeatherApp
{
    public static class Startup
    {
        public static IServiceProvider ServiceProvider { get; set; }
        public static App Init(Action<HostBuilderContext, IServiceCollection> nativeConfigureServices)
        {
            var a = Assembly.GetExecutingAssembly();
            var stream = a.GetManifestResourceStream("LocalWeatherApp.appsettings.json");

            var host = new HostBuilder()
                        .ConfigureHostConfiguration(c =>
                        {
                            c.AddCommandLine(new string[] { $"ContentRoot={FileSystem.AppDataDirectory}" });
                            c.AddJsonStream(stream);
                        })
                        .ConfigureServices((c, x) =>
                        {
                            nativeConfigureServices(c, x);
                            ConfigureServices(c, x);
                        })
                        .ConfigureLogging(l => l.AddConsole(o =>
                        {
                            o.DisableColors = true;
                        }))
                        .Build();
            App.ServiceProvider = host.Services;

            return App.ServiceProvider.GetService<App>();
        }

        static void ConfigureServices(HostBuilderContext ctx, IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddTransient<MainPage>();
            services.AddSingleton<App>();
            services.AddSingleton<IWeatherServiceClient, WeatherServiceClient>();
            services.AddTransient<IWeatherLocationDataRetriever, WeatherLocationDataRetriever>();
            services.AddTransient<IWeatherLocationDetailDataRetriever,WeatherLocationDetailDataRetriever>();
            services.AddTransient<IWeatherViewModel>(s => new WeatherViewModel(s.GetService <IWeatherLocationDataRetriever>()));
            services.AddTransient(s => new WeatherDetailViewModel(s.GetService<IWeatherLocationDetailDataRetriever>()));
            services.AddTransient(s => new WeatherPage(s.GetService<IWeatherViewModel>(), s.GetService<IWeatherLocationDetailDataRetriever>()));
            services.AddTransient(s => new WeatherDetailPage(s.GetService<IWeatherDetailViewModel>()));
            services.AddTransient(s => new AboutPage());

            //// The HostingEnvironment comes from the appsetting.json and could be optionally used to configure the mock service
            //if (ctx.HostingEnvironment.IsDevelopment())
            //{
            
            //}
            //else
            //{
            
            //}
        }
    }
}