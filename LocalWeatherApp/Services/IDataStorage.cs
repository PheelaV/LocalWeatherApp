namespace LocalWeatherApp.Services
{
    public interface IDataStorage<T> : IDataRetriever<T>, IDataUpdater<T>
    {
    }
}
