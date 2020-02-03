using LocalWeatherApp.ViewModels.Weather;
using System.ComponentModel;
using LocalWeatherApp.Helpers;
using Xamarin.Forms;

namespace LocalWeatherApp.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class WeatherDetailPage : ContentPage
    {
        readonly IWeatherDetailViewModel viewModel;

        public WeatherDetailPage(IWeatherDetailViewModel viewModel)
        {
            this.InitializeComponent();

            this.BindingContext = this.viewModel = viewModel.CheckForNull();
        }
    }
}