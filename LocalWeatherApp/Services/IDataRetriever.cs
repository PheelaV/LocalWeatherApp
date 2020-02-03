using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocalWeatherApp.Services
{
    public interface IDataRetriever<T>
    {
        Task<T> GetItemAsync(string id);
        Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false);
    }
}
