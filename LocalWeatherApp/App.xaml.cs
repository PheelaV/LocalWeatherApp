using LocalWeatherApp.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LocalWeatherApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; set; }

        public App()
        {
            this.InitializeComponent();
            this.MainPage = ServiceProvider.GetService<MainPage>();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
