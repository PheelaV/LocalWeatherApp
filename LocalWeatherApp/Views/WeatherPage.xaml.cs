using LocalWeatherApp.Helpers;
using LocalWeatherApp.Services.WeatherService;
using LocalWeatherApp.Services.WeatherService.Models;
using LocalWeatherApp.ViewModels.Weather;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LocalWeatherApp.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class WeatherPage : ContentPage
    {
        readonly IWeatherViewModel viewModel;

        private IWeatherLocationDetailDataRetriever DataRetriever { get; set; }

        public WeatherPage(IWeatherViewModel weatherViewModel, IWeatherLocationDetailDataRetriever weatherLocationDetailDataRetriever)
        {
            this.InitializeComponent();

            this.BindingContext = this.viewModel = weatherViewModel.CheckForNull();
            this.DataRetriever = weatherLocationDetailDataRetriever.CheckForNull();

            this.WeatherLocationSearchesListView.ItemsSource = this.viewModel.Items;
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            //var weatherLocation = (WeatherLocation) sender;
            var item = args.SelectedItem as WeatherLocation;
            if (!(item != null))
                return;
            var model = App.ServiceProvider.GetService<WeatherDetailViewModel>().CheckForNull();
            // scetchy, maybe pass only dto?
            model.LoadItemsCommand.Execute(item.Woeid);

            await this.Navigation.PushAsync(new WeatherDetailPage(model));

            // Manually deselect item.
            this.WeatherLocationSearchesListView.SelectedItem = null;
        }

        protected void Refresh_Clicked(object sender, EventArgs e)
        {
            this.viewModel.LoadItemsCommand.Execute(null);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }


    }
}