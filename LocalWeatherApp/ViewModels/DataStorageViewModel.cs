using LocalWeatherApp.Services;
using Xamarin.Forms;

namespace LocalWeatherApp.ViewModels
{
    public abstract class DataStorageViewModel<T> : BaseViewModel<T>
    {
        public IDataStorage<T> DataStorage => DependencyService.Get<IDataStorage<T>>();
    }
}
