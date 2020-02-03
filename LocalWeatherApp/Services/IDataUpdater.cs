using System.Threading.Tasks;

namespace LocalWeatherApp.Services
{
    public interface IDataUpdater<in T>
    {
        Task<bool> AddItemAsync(T item);
        Task<bool> UpdateItemAsync(T item);
        Task<bool> DeleteItemAsync(string id);
    }
}
