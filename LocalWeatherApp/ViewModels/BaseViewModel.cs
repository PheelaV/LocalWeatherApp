using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LocalWeatherApp.ViewModels
{
    public interface IBaseViewModel
    {
        bool IsBusy { get; set; }
        string Title { get; set; }
        event PropertyChangedEventHandler PropertyChanged;
    }

    public abstract class BaseViewModel<T> : INotifyPropertyChanged, IBaseViewModel
    {
        bool isBusy = false;
        public bool IsBusy
        {
            get => this.isBusy;
            set => this.SetProperty(ref this.isBusy, value);
        }

        private string title = string.Empty;
        public string Title
        {
            get => this.title;
            set => this.SetProperty(ref this.title, value);
        }

        protected bool SetProperty<P>(ref P backingStore, P value,
            [CallerMemberName]string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<P>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            this.OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = this.PropertyChanged;

            changed?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
