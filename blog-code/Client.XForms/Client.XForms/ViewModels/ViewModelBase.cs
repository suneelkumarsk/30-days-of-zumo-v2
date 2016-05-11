using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Client.XForms.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public ViewModelBase()
        {

        }

        #region Properties
        #region Title Property
        private string _title = string.Empty;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value, "Title"); }
        }
        #endregion

        #region Subtitle Property
        private string _subtitle = string.Empty;
        public string Subtitle
        {
            get { return _subtitle; }
            set { SetProperty(ref _subtitle, value, "Subtitle"); }
        }
        #endregion

        #region Icon Property
        private string _icon = null;
        public string Icon
        {
            get { return _icon; }
            set { SetProperty(ref _icon, value, "Icon"); }
        }
        #endregion

        #region IsBusy Property
        private bool _busy = false;
        public bool IsBusy
        {
            get { return _busy; }
            set { SetProperty(ref _busy, value, "IsBusy"); }
        }
        #endregion
        #endregion

        #region SetProperty
        protected void SetProperty<T>(ref T backingStore, T value, string propName, Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return;
            backingStore = value;
            if (onChanged != null)
                onChanged();
            OnPropertyChanged(propName);
        }
        #endregion

        #region INotifyPropertyChanged Interface
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propName)
        {
            if (PropertyChanged == null)
                return;
            PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
        #endregion

        public void DisplayAlert(string title, string message, string dismiss)
        {
            Acr.UserDialogs.UserDialogs.Instance.ShowError(message);
        }
    }
}
