using LocalWeatherApp.Configuration;
using LocalWeatherApp.Services.WeatherService;
using LocalWeatherApp.Services.WeatherService.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LocalWeatherApp.ViewModels.Weather
{
    public interface IWeatherDetailViewModel
    {
        ObservableCollection<ConsolidatedWeather> ConsolidatedWeathers { get; set; }
        ConsolidatedWeather WeatherToday { get; set; }
        ConsolidatedWeather WeatherTomorrow { get; set; }
        Uri WeatherTomorrowStateURL { get; set; }
        Uri WeatherTodayStateURL { get; set; }
        WeatherLocationDetail Item { get; set; }
        WeatherLocation WeatherLocation { get; set; }
        Command LoadItemsCommand { get; set; }
        bool IsBusy { get; set; }
        string Title { get; set; }
        event PropertyChangedEventHandler PropertyChanged;
    }

    public class WeatherDetailViewModel : BaseViewModel<WeatherLocationDetail>, IWeatherDetailViewModel
    {
        public ObservableCollection<ConsolidatedWeather> ConsolidatedWeathers { get; set; }

        private ConsolidatedWeather weatherToday;
        public ConsolidatedWeather WeatherToday 
        { 
            get => this.weatherToday; 
            set => this.SetProperty(ref this.weatherToday, value);
        }

        private ConsolidatedWeather weatherTomorrow;
        public ConsolidatedWeather WeatherTomorrow
        {
            get => this.weatherTomorrow;
            set => this.SetProperty(ref this.weatherTomorrow, value);
        }

        private Uri weatherTomorrowStateURL;
        public Uri WeatherTomorrowStateURL
        {
            get => this.weatherTomorrowStateURL;
            set => this.SetProperty(ref this.weatherTomorrowStateURL, value);
        }

        private Uri weatherTodayStateURL;
        public Uri WeatherTodayStateURL
        {
            get => this.weatherTodayStateURL;
            set => this.SetProperty(ref this.weatherTodayStateURL, value);
        }

        public WeatherLocationDetail Item { get; set; }
        public WeatherLocation WeatherLocation { get; set; }
        public Command LoadItemsCommand { get; set; }

        private IWeatherLocationDetailDataRetriever DataRetriever { get; set; }

        public WeatherDetailViewModel(IWeatherLocationDetailDataRetriever dataRetriever)
        {
            this.DataRetriever = dataRetriever;
            this.WeatherToday = new ConsolidatedWeather() { MaxTemp = 0.00f, MinTemp = 0.00f };
            this.weatherTomorrow = new ConsolidatedWeather() { MaxTemp = 0.00f, MinTemp = 0.00f };

            this.LoadItemsCommand = new Command(async (woeid) => await this.ExecuteLoadItemsCommand(woeid.ToString()));
        }

        async Task ExecuteLoadItemsCommand(string woeid)
        {
            if (this.IsBusy)
                return;

            this.IsBusy = true;

            try
            {
                var item = await this.DataRetriever.GetItemAsync(woeid);
                this.Item = item;
                this.WeatherToday = item.ConsolidatedWeather?[0];
                this.WeatherTomorrow = item.ConsolidatedWeather?[1];

                var weatherStateIconEP = AppSettingsManager.Settings["WeatherService"] + "/static/img/weather/png/{0}.png";
                this.WeatherTodayStateURL = new Uri(string.Format(weatherStateIconEP, this.WeatherToday?.WeatherStateAbbr));
                this.WeatherTomorrowStateURL = new Uri(string.Format(weatherStateIconEP, this.weatherTomorrow?.WeatherStateAbbr));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                this.IsBusy = false;
            }
        }
    }
}
