using LocalWeatherApp.Services;

namespace LocalWeatherApp.ViewModels
{
    public interface IRetrievingBaseViewModel<T> : IBaseViewModel
    {
        IDataRetriever<T> DataRetriever { get; set; }
    }

    //    public abstract class RetrievingBaseViewModel<T> : BaseViewModel<T>, IRetrievingBaseViewModel<T>
    //    {
    //        public IDataRetriever<T> DataRetriever { get; set; }

    //        protected RetrievingBaseViewModel(IDataRetriever<T> dataRetriever)
    //        {
    //            this.DataRetriever = dataRetriever;
    //        }

    //    }
}
