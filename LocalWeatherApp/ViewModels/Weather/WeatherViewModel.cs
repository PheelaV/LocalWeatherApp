using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LocalWeatherApp.Services.WeatherService;
using LocalWeatherApp.Services.WeatherService.Models;
using Xamarin.Forms;

namespace LocalWeatherApp.ViewModels.Weather
{
    public interface IWeatherViewModel
    {
        ObservableCollection<WeatherLocation> Items { get; set; }
        Command LoadItemsCommand { get; set; }
        bool IsBusy { get; set; }
        string Title { get; set; }
        event PropertyChangedEventHandler PropertyChanged;
    }

    public class WeatherViewModel : BaseViewModel<WeatherLocation>, IWeatherViewModel
    {
        public ObservableCollection<WeatherLocation> Items { get; set; }
        public Command LoadItemsCommand { get; set; }

        private IWeatherLocationDataRetriever DataRetriever { get; set; }

        public WeatherViewModel(IWeatherLocationDataRetriever dataRetriever)
        {
            this.DataRetriever = dataRetriever;
            this.Title = "Weather";
            this.Items = new ObservableCollection<WeatherLocation>();
            this.LoadItemsCommand = new Command(async () => await this.ExecuteLoadItemsCommand());

            //TODO: The instruction specifically say not to populate the app with data prior to the users refresh input
            //this.LoadItemsCommand.Execute(null);
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (this.IsBusy)
                return;

            this.IsBusy = true;

            try
            {
                this.Items.Clear();
                var items = (await this.DataRetriever.GetItemsAsync(true)).OrderBy(x => x.Title);
                foreach (var item in items)
                {
                    this.Items.Add(item);
                }
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